using System;
using Enigma.Reflection;
using Enigma.Reflection.Emit;
using MethodBuilder = Enigma.Reflection.Emit.MethodBuilder;

namespace Enigma.Serialization.Reflection.Emit
{
    public class DynamicWriteTravellerBuilder
    {
        private static readonly DynamicWriteTravellerMembers Members = new DynamicWriteTravellerMembers();

        private readonly ILCodeVariable _visitorVariable;
        private readonly SerializableType _target;
        private readonly TravellerContext _context;
        private readonly ILExpressed _il;

        public DynamicWriteTravellerBuilder(MethodBuilder builder, SerializableType target, TravellerContext context)
        {
            _target = target;
            _context = context;
            _il = builder.IL;
            _visitorVariable = new MethodArgILCodeVariable(1, typeof(IWriteVisitor));
        }

        public void BuildTravelWriteMethod()
        {
            var graphArgument = new MethodArgILCodeVariable(2, _target.Type);
                foreach (var property in _target.Properties) {
                GeneratePropertyCode(graphArgument, property);
            }
            _il.Return();
        }

        private void GeneratePropertyCode(ILCodeVariable graphVariable, SerializableProperty target)
        {
            var extPropertyType = target.Ext;
            var argsField = _context.GetArgsField(target);
            var argsFieldVariable = _il.Var.Field(_il.Var.This(), argsField);
            if (target.Ext.IsValueOrNullableOfValue()) {
                var valueType = target.Ext.IsEnum()
                    ? target.Ext.GetUnderlyingEnumType()
                    : target.Ref.PropertyType;

                var propertyParameter = _il.Var.Property(graphVariable, target.Ref).AsNullable();

                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorVisitValue[valueType], propertyParameter, argsFieldVariable);
            }
            else if (extPropertyType.Class == TypeClass.Dictionary) {
                var container = extPropertyType.Container.AsDictionary();

                var dictionaryType = container.DictionaryInterfaceType;
                var cLocal = _il.DeclareLocal("dictionary", dictionaryType);
                _il.Snippets.SetVariable(cLocal, _il.Var.Property(graphVariable, target.Ref).Cast(dictionaryType));

                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorVisit, cLocal, argsFieldVariable);

                _il.IfNotEqual(cLocal, null)
                    .Then(() => GenerateDictionaryCode(cLocal, container.ElementType))
                    .End();

                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorLeave, cLocal, argsFieldVariable);
            }
            else if (extPropertyType.Class == TypeClass.Collection) {
                var container = extPropertyType.Container.AsCollection();

                var collectionType = extPropertyType.Ref.IsArray && extPropertyType.Container.AsArray().Ranks > 1
                    ? extPropertyType.Ref
                    : container.CollectionInterfaceType;

                var cLocal = _il.DeclareLocal("collection", collectionType);

                _il.Snippets.SetVariable(cLocal, _il.Var.Property(graphVariable, target.Ref).Cast(collectionType));

                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorVisit, cLocal, argsFieldVariable);

                _il.IfNotEqual(cLocal, null).Then(
                    () => GenerateEnumerateCollectionContentCode(extPropertyType, cLocal))
                    .End();

                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorLeave, cLocal, argsFieldVariable);
            }
            else {
                var singleLocal = _il.DeclareLocal("single", target.Ref.PropertyType);
                _il.Snippets.GetPropertyValue(graphVariable, target.Ref);
                _il.Var.Set(singleLocal);

                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorVisit, singleLocal, argsFieldVariable);

                var checkIfNullLabel = _il.DefineLabel();
                _il.TransferIfNull(singleLocal, checkIfNullLabel);

                GenerateChildCall(singleLocal);

                _il.MarkLabel(checkIfNullLabel);

                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorLeave, singleLocal, argsFieldVariable);
            }
        }

        private void GenerateDictionaryCode(ILCodeVariable dictionary, Type elementType)
        {
            var enumerateCode = new ILEnumerateCode(dictionary, (il, it) => {
                GenerateEnumerateContentCode(new InstancePropertyILCodeVariable(it, elementType.GetProperty("Key")), LevelType.DictionaryKey);
                GenerateEnumerateContentCode(new InstancePropertyILCodeVariable(it, elementType.GetProperty("Value")), LevelType.DictionaryValue);
            });
            _il.Generate(enumerateCode);
        }

        private void GenerateEnumerateContentCode(ILCodeParameter valueParam, LevelType level)
        {
            var type = valueParam.ParameterType;
            var extType = _il.TypeCache.Extend(type);

            var visitArgs = GetContentVisitArgs(extType, level);

            if (extType.IsValueOrNullableOfValue()) {
                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorVisitValue[type], valueParam.AsNullable(), visitArgs);
            }
            else if (extType.Class == TypeClass.Dictionary) {
                var container = extType.Container.AsDictionary();
                var elementType = container.ElementType;

                var dictionaryType = container.DictionaryInterfaceType;

                var dictionaryLocal = _il.DeclareLocal("dictionary", dictionaryType);
                _il.Snippets.SetVariable(dictionaryLocal, valueParam.Cast(dictionaryType));

                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorVisit, dictionaryLocal, visitArgs);

                var part = new ILEnumerateCode(dictionaryLocal, (il, it) => {
                    GenerateEnumerateContentCode(new InstancePropertyILCodeVariable(it, elementType.GetProperty("Key")), LevelType.DictionaryKey);
                    GenerateEnumerateContentCode(new InstancePropertyILCodeVariable(it, elementType.GetProperty("Value")), LevelType.DictionaryValue);
                });
                _il.Generate(part);

                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorLeave, dictionaryLocal, visitArgs);
            }
            else if (extType.Class == TypeClass.Collection) {
                var container = extType.Container.AsCollection();
                var collectionType = type.IsArray && extType.Container.AsArray().Ranks > 1
                    ? type
                    : container.CollectionInterfaceType;

                var collectionLocal = _il.DeclareLocal("collection", collectionType);
                _il.Snippets.SetVariable(collectionLocal, valueParam.Cast(collectionType));

                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorVisit, collectionLocal, visitArgs);

                GenerateEnumerateCollectionContentCode(extType, collectionLocal);

                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorLeave, collectionLocal, visitArgs);
            }
            else {
                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorVisit, valueParam, visitArgs);

                GenerateChildCall(valueParam);

                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorLeave, valueParam, visitArgs);
            }
        }

        private void GenerateEnumerateCollectionContentCode(ExtendedType target, ILCodeParameter collectionParameter)
        {
            ArrayContainerTypeInfo arrayTypeInfo;
            if (target.TryGetArrayTypeInfo(out arrayTypeInfo) && arrayTypeInfo.Ranks > 1) {
                if (arrayTypeInfo.Ranks > 3)
                    throw new NotSupportedException("The serialization engine is limited to 3 ranks in arrays");

                _il.Snippets.ForLoop(0, new CallMethodILCode(collectionParameter, Members.ArrayGetLength, 0), 1,
                    r0 => {
                        _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorVisit, collectionParameter, Members.VisitArgsCollectionInCollection);
                        _il.Snippets.ForLoop(0, new CallMethodILCode(collectionParameter, Members.ArrayGetLength, 1), 1,
                            r1 => {
                                if (arrayTypeInfo.Ranks > 2) {
                                    _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorVisit, collectionParameter, Members.VisitArgsCollectionInCollection);

                                    _il.Snippets.ForLoop(0, new CallMethodILCode(collectionParameter, Members.ArrayGetLength, 1), 1,
                                        r2 => GenerateEnumerateContentCode(
                                            new CallMethodILCode(collectionParameter, target.Ref.GetMethod("Get"), r0, r1, r2),
                                            LevelType.CollectionItem));

                                    _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorLeave, collectionParameter, Members.VisitArgsCollectionInCollection);
                                }
                                else {
                                    GenerateEnumerateContentCode(new CallMethodILCode(collectionParameter, target.Ref.GetMethod("Get"), r0, r1), LevelType.CollectionItem);
                                }
                            });
                        _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorLeave, collectionParameter, Members.VisitArgsCollectionInCollection);
                    });
            }
            else {
                var part = new ILEnumerateCode(collectionParameter, (il, it) => GenerateEnumerateContentCode(it, LevelType.CollectionItem));
                _il.Generate(part);
            }
        }

        private ILCodeParameter GetContentVisitArgs(ExtendedType type, LevelType level)
        {
            if (!type.IsValueOrNullableOfValue()) {
                if (type.Class == TypeClass.Dictionary) {
                    if (level == LevelType.DictionaryKey)
                        return Members.VisitArgsDictionaryInDictionaryKey;
                    if (level == LevelType.DictionaryValue)
                        return Members.VisitArgsDictionaryInDictionaryValue;
                    return Members.VisitArgsDictionaryInCollection;
                }

                if (type.Class == TypeClass.Collection) {
                    if (level == LevelType.DictionaryKey)
                        return Members.VisitArgsCollectionInDictionaryKey;
                    
                    if (level == LevelType.DictionaryValue)
                        return Members.VisitArgsCollectionInDictionaryValue;
                    
                    return Members.VisitArgsCollectionInCollection;
                }
            }

            if (level == LevelType.DictionaryKey)
                return Members.VisitArgsDictionaryKey;
            
            if (level == LevelType.DictionaryValue)
                return Members.VisitArgsDictionaryValue;
            
            return Members.VisitArgsCollectionItem;
        }

        private void GenerateChildCall(ILCodeParameter child)
        {
            var childTravellerInfo = _context.GetTraveller(child.ParameterType);

            var field = _il.Var.Field(_il.Var.This(), childTravellerInfo.Field);
            _il.Snippets.InvokeMethod(field, childTravellerInfo.TravelWriteMethod, _visitorVariable, child);
        }

        
    }
}