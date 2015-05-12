using System;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
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
            //if (extPropertyType.Class == TypeClass.Dictionary) return;
            //if (extPropertyType.Class == TypeClass.Complex) return;
            //if (extPropertyType.Class == TypeClass.Collection) return;
            //if (extPropertyType.Class == TypeClass.Nullable) return;
            //if (extPropertyType.Class == TypeClass.Value) return;
            if (extPropertyType.IsValueOrNullableOfValue()) {
                var isNullable = extPropertyType.Class == TypeClass.Nullable;
                var isEnum = extPropertyType.IsEnum();

                var valueType = extPropertyType.Inner;

                _il.Var.Load(_visitorVariable);
                _il.Snippets.GetPropertyValue(graphVariable, target.Ref);

                if (isEnum) {
                    valueType = extPropertyType.GetUnderlyingEnumType();
                    if (!isNullable) _il.Snippets.AsNullable(valueType);
                }
                else if (!isNullable && valueType.IsValueType)
                    _il.Snippets.AsNullable(valueType);

                _il.Var.Load(argsFieldVariable);

                var visitValueMethod = Members.VisitorVisitValue[valueType];
                _il.CallVirt(visitValueMethod);
            }
            else if (extPropertyType.Class == TypeClass.Dictionary) {
                var container = extPropertyType.Container.AsDictionary();
                _il.Snippets.GetPropertyValue(graphVariable, target.Ref);

                var dictionaryType = container.DictionaryInterfaceType;
                var cLocal = _il.DeclareLocal("dictionary", dictionaryType);
                if (dictionaryType != extPropertyType.Inner)
                    _il.Cast(dictionaryType);
                _il.Var.Set(cLocal);

                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorVisit, cLocal, argsFieldVariable);

                GenerateDictionaryCode(cLocal, container.ElementType);

                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorLeave, cLocal, argsFieldVariable);
            }
            else if (extPropertyType.Class == TypeClass.Collection) {
                var container = extPropertyType.Container.AsCollection();
                _il.Snippets.GetPropertyValue(graphVariable, target.Ref);

                var collectionType = container.CollectionInterfaceType;
                var cLocal = _il.DeclareLocal("collection", collectionType);
                if (collectionType != extPropertyType.Inner)
                    _il.Cast(collectionType);
                _il.Var.Set(cLocal);

                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorVisit, cLocal, argsFieldVariable);

                GenerateCollectionCode(cLocal);

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
            var transferIfNullLabel = _il.DefineLabel();
            _il.TransferIfNull(dictionary, transferIfNullLabel);

            var enumerateCode = new ILEnumerateCode(dictionary, (il, it) => {
                GenerateEnumerateContentCode(new InstancePropertyILCodeVariable(it, elementType.GetProperty("Key")), Members.VisitArgsDictionaryKey);
                GenerateEnumerateContentCode(new InstancePropertyILCodeVariable(it, elementType.GetProperty("Value")), Members.VisitArgsDictionaryValue);
            });
            _il.Generate(enumerateCode);

            _il.MarkLabel(transferIfNullLabel);
        }

        private void GenerateCollectionCode(ILCodeVariable collection)
        {
            var transferIfNullLabel = _il.DefineLabel();
            _il.TransferIfNull(collection, transferIfNullLabel);

            var enumerateCode = new ILEnumerateCode(collection, (il, it) => GenerateEnumerateContentCode(it, Members.VisitArgsCollectionItem));
            _il.Generate(enumerateCode);

            _il.MarkLabel(transferIfNullLabel);
        }

        private void GenerateEnumerateContentCode(ILCodeVariable variable, FieldInfo staticVisitArgsField)
        {
            var type = variable.VariableType;
            var extType = _il.TypeCache.Extend(type);

            if (extType.IsValueOrNullableOfValue()) {
                var isNullable = extType.Class == TypeClass.Nullable;
                _il.Var.Load(_visitorVariable);
                _il.Var.Load(variable);
                if (!isNullable && type.IsValueType) {
                    var container = _il.TypeCache.Extend(type.AsNullable())
                        .Container.AsNullable();

                    _il.Construct(container.Constructor);
                }
                _il.LoadStaticField(staticVisitArgsField);
                var visitValueMethod = Members.VisitorVisitValue[type];
                _il.CallVirt(visitValueMethod);
            }
            else if (extType.Class == TypeClass.Dictionary) {
                var container = extType.Container.AsDictionary();
                var elementType = container.ElementType;

                var dictionaryType = container.DictionaryInterfaceType;

                if (type == dictionaryType) {
                    var enumerableCode = new ILEnumerateCode(variable, (il, it) => {
                        GenerateEnumerateContentCode(new InstancePropertyILCodeVariable(it, elementType.GetProperty("Key")), Members.VisitArgsDictionaryKey);
                        GenerateEnumerateContentCode(new InstancePropertyILCodeVariable(it, elementType.GetProperty("Value")), Members.VisitArgsDictionaryValue);
                    });
                    _il.Generate(enumerableCode);
                }
                else {
                    _il.Var.Load(variable);
                    _il.Cast(dictionaryType);
                    var dictionaryLocal = _il.DeclareLocal("dictionary", dictionaryType);
                    _il.Var.Set(dictionaryLocal);

                    var part = new ILEnumerateCode(dictionaryLocal, (il, it) => {
                        GenerateEnumerateContentCode(new InstancePropertyILCodeVariable(it, elementType.GetProperty("Key")), Members.VisitArgsDictionaryKey);
                        GenerateEnumerateContentCode(new InstancePropertyILCodeVariable(it, elementType.GetProperty("Value")), Members.VisitArgsDictionaryValue);
                    });
                    _il.Generate(part);
                }
            }
            else if (extType.Class == TypeClass.Collection) {
                var container = extType.Container.AsCollection();
                var collectionType = container.CollectionInterfaceType;
                if (type == collectionType) {
                    var part = new ILEnumerateCode(variable, (il, it) => GenerateEnumerateContentCode(it, Members.VisitArgsCollectionItem));
                    _il.Generate(part);
                }
                else {
                    _il.Var.Load(variable);
                    _il.Cast(collectionType);
                    var collectionLocal = _il.DeclareLocal("collection", collectionType);
                    _il.Var.Set(collectionLocal);

                    var part = new ILEnumerateCode(collectionLocal, (il, it) => GenerateEnumerateContentCode(it, Members.VisitArgsCollectionItem));
                    _il.Generate(part);
                }
            }
            else {
                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorVisit, variable, _il.Var.Field(staticVisitArgsField));

                GenerateChildCall(variable);

                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorLeave, variable, _il.Var.Field(staticVisitArgsField));
            }
        }

        private void GenerateChildCall(ILCodeVariable child)
        {
            var childTravellerInfo = _context.GetTraveller(child.VariableType);

            //if (child.VariableType.Name == "Identifier") {
            //    var loc = _il.DeclareLocal("Test", child.VariableType);
            //    //_il.Construct(child.VariableType.GetConstructor(new Type[] { }));
            //    _il.Var.Load(child);
            //    _il.Var.Set(loc);
            //    return;

            //}

            ////var field = _il.Var.Field(_il.Var.This(), childTravellerInfo.Field);
            ////_il.LoadThis();
            ////_il.LoadField(childTravellerInfo.Field);
            ////_il.Var.Load(field);
            ////_il.Var.Load(_visitorVariable);
            ////_il.Var.Load(child);
            //_il.LoadThis();
            //_il.LoadField(childTravellerInfo.Field);
            //_il.Var.Load(_visitorVariable);
            ////_il.Var.Load(loc);
            ////_il.Construct(child.VariableType.GetConstructor(new Type[] { }));
            ////_il.LoadNull();
            //_il.Var.Load(child);

            //_il.CallVirt(childTravellerInfo.TravelWriteMethod);

            var field = _il.Var.Field(_il.Var.This(), childTravellerInfo.Field);
            _il.Snippets.InvokeMethod(field, childTravellerInfo.TravelWriteMethod, _visitorVariable, child);
        }

        
    }
}