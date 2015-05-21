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

                var collectionType = container.CollectionInterfaceType;
                var cLocal = _il.DeclareLocal("collection", collectionType);

                _il.Snippets.SetVariable(cLocal, _il.Var.Property(graphVariable, target.Ref).Cast(collectionType));

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
            var enumerateCode = new ILEnumerateCode(dictionary, (il, it) => {
                GenerateEnumerateContentCode(new InstancePropertyILCodeVariable(it, elementType.GetProperty("Key")), Members.VisitArgsDictionaryKey);
                GenerateEnumerateContentCode(new InstancePropertyILCodeVariable(it, elementType.GetProperty("Value")), Members.VisitArgsDictionaryValue);
            });
            _il.Generate(enumerateCode);
        }

        private void GenerateCollectionCode(ILCodeVariable collection)
        {
            var transferIfNullLabel = _il.DefineLabel();
            _il.TransferIfNull(collection, transferIfNullLabel);

            var enumerateCode = new ILEnumerateCode(collection, (il, it) => GenerateEnumerateContentCode(it, Members.VisitArgsCollectionItem));
            _il.Generate(enumerateCode);

            _il.MarkLabel(transferIfNullLabel);
        }

        private void GenerateEnumerateContentCode(ILCodeVariable variable, ILCodeParameter visitArgsParameter)
        {
            var type = variable.VariableType;
            var extType = _il.TypeCache.Extend(type);

            if (extType.IsValueOrNullableOfValue()) {
                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorVisitValue[type], variable.AsNullable(), visitArgsParameter);
            }
            else if (extType.Class == TypeClass.Dictionary) {
                var container = extType.Container.AsDictionary();
                var elementType = container.ElementType;

                var dictionaryType = container.DictionaryInterfaceType;

                var dictionaryLocal = _il.DeclareLocal("dictionary", dictionaryType);
                _il.Snippets.SetVariable(dictionaryLocal, variable.Cast(dictionaryType));

                var part = new ILEnumerateCode(dictionaryLocal, (il, it) => {
                    GenerateEnumerateContentCode(new InstancePropertyILCodeVariable(it, elementType.GetProperty("Key")), Members.VisitArgsDictionaryKey);
                    GenerateEnumerateContentCode(new InstancePropertyILCodeVariable(it, elementType.GetProperty("Value")), Members.VisitArgsDictionaryValue);
                });
                _il.Generate(part);
            }
            else if (extType.Class == TypeClass.Collection) {
                var container = extType.Container.AsCollection();
                var collectionType = container.CollectionInterfaceType;

                var collectionLocal = _il.DeclareLocal("collection", collectionType);
                _il.Snippets.SetVariable(collectionLocal, variable.Cast(collectionType));

                var part = new ILEnumerateCode(collectionLocal, (il, it) => GenerateEnumerateContentCode(it, Members.VisitArgsCollectionItem));
                _il.Generate(part);
            }
            else {
                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorVisit, variable, visitArgsParameter);

                GenerateChildCall(variable);

                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorLeave, variable, visitArgsParameter);
            }
        }

        private void GenerateChildCall(ILCodeVariable child)
        {
            var childTravellerInfo = _context.GetTraveller(child.VariableType);

            var field = _il.Var.Field(_il.Var.This(), childTravellerInfo.Field);
            _il.Snippets.InvokeMethod(field, childTravellerInfo.TravelWriteMethod, _visitorVariable, child);
        }

        
    }
}