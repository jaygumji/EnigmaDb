using System;
using System.Reflection;
using System.Reflection.Emit;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace Enigma.Reflection.Emit
{
    public class ILExpressed
    {
        private readonly ILGenerator _il;

        public ILExpressed(ILGenerator il)
        {
            _il = il;
        }

        public ILGenerator Gen { get { return _il; } }

        public void LoadArgs(params int[] indexes)
        {
            foreach (var index in indexes) {
                OpCode opCode;
                if (OpCodesLookups.LoadArg.TryGetValue(index, out opCode))
                    _il.Emit(opCode);
                else
                    _il.Emit(OpCodes.Ldarg_S, index);
            }
        }

        public void LoadArgsAddress(params int[] indexes)
        {
            foreach (var index in indexes)
                _il.Emit(OpCodes.Ldarga_S, index);
        }

        public void LoadVal(int value)
        {
            OpCode opCode;
            if (OpCodesLookups.LoadInt32.TryGetValue(value, out opCode)) {
                _il.Emit(opCode);
                return;
            }

            _il.Emit(OpCodes.Ldc_I4_S, value);
        }

        public void LoadVal(uint value)
        {
            LoadVal((int)value);
        }

        public void LoadVal(string value)
        {
            _il.Emit(OpCodes.Ldstr, value);
        }

        public void LoadVar(IVariable variable)
        {
            if (variable == null) throw new ArgumentNullException("variable");

            var local = variable as LocalVariable;
            if (local != null) {
                LoadLocal(local.Local);
                return;
            }
            var arg = variable as MethodArgVariable;
            if (arg != null) {
                LoadArgs(arg.Index);
                return;
            }
            var property = variable as InstancePropertyVariable;
            if (property != null) {
                if (property.Instance.VariableType.IsValueType)
                    LoadVarAddress(property.Instance);
                else
                    LoadVar(property.Instance);

                if (property.VariableType.IsValueType)
                    Call(property.Info.GetGetMethod());
                else
                    CallVirt(property.Info.GetGetMethod());
                return;
            }

            throw new InvalidOperationException("Invalid variable " + variable.GetType());
        }

        public void LoadVarAddress(IVariable variable)
        {
            if (variable == null) throw new ArgumentNullException("variable");

            var local = variable as LocalVariable;
            if (local != null) {
                LoadLocalAddress(local.Local);
                return;
            }
            var arg = variable as MethodArgVariable;
            if (arg != null) {
                _il.Emit(OpCodes.Ldarga_S, arg.Index);
                LoadArgsAddress(arg.Index);
                return;
            }

            throw new InvalidOperationException("Invalid variable " + variable.GetType());
        }

        public void Call(MethodInfo method)
        {
            _il.EmitCall(OpCodes.Call, method, null);
        }

        public void CallVirt(MethodInfo method)
        {
            _il.EmitCall(OpCodes.Callvirt, method, null);
        }

        public void Return()
        {
            _il.Emit(OpCodes.Ret);
        }

        public void Cast(Type type)
        {
            _il.Emit(OpCodes.Castclass, type);
        }

        public LocalBuilder DeclareLocal(string name, Type type)
        {
            //int index;
            //if (_locals.TryGetValue(name, out index)) {
            //    _locals[name] = ++index;
            //}
            //else {
            //    index = 0;
            //    _locals.Add(name, index);
            //}
            var local = _il.DeclareLocal(type);
            return local;
        }

        public void SetLocal(LocalBuilder local)
        {
            OpCode opCode;
            if (OpCodesLookups.SetLocal.TryGetValue(local.LocalIndex, out opCode)) {
                _il.Emit(opCode);
                return;
            }

            _il.Emit(OpCodes.Stloc_S, local.LocalIndex);
        }

        public void LoadLocalAddress(LocalBuilder local)
        {
            _il.Emit(OpCodes.Ldloca_S, local.LocalIndex);
        }

        public void LoadLocal(LocalBuilder local)
        {
            OpCode opCode;
            if (OpCodesLookups.GetLocal.TryGetValue(local.LocalIndex, out opCode)) {
                _il.Emit(opCode);
                return;
            }
            _il.Emit(OpCodes.Ldloc_S, local.LocalIndex);
        }

        public Label DefineLabel()
        {
            return _il.DefineLabel();
        }

        public void MarkLabel(Label label)
        {
            _il.MarkLabel(label);
        }

        public void LoadField(FieldInfo field)
        {
            _il.Emit(OpCodes.Ldfld, field);
        }

        public void LoadStaticField(FieldInfo field)
        {
            _il.Emit(OpCodes.Ldsfld, field);
        }

        public void TransferLong(Label label)
        {
            _il.Emit(OpCodes.Br, label);
        }

        public void TransferLongIfFalse(Label label)
        {
            _il.Emit(OpCodes.Brfalse, label);
        }

        public void TransferLongIfTrue(Label label)
        {
            _il.Emit(OpCodes.Brtrue, label);
        }

        public void TransferShort(Label label)
        {
            _il.Emit(OpCodes.Br_S, label);
        }

        public void TransferShortIfFalse(Label label)
        {
            _il.Emit(OpCodes.Brfalse_S, label);
        }

        public void TransferShortIfTrue(Label label)
        {
            _il.Emit(OpCodes.Brtrue_S, label);
        }

        public Label Try()
        {
            return _il.BeginExceptionBlock();
        }

        public void Catch(Type exceptionType)
        {
            _il.BeginCatchBlock(exceptionType);
        }

        public void Finally()
        {
            _il.BeginFinallyBlock();
        }

        public void EndTry()
        {
            _il.EndExceptionBlock();
        }

        public void LoadNull()
        {
            _il.Emit(OpCodes.Ldnull);
        }

        public void CompareEquals()
        {
            _il.Emit(OpCodes.Ceq);
        }

        public void SetFieldWithDefaultConstructor(FieldInfo field, ConstructorInfo constructor)
        {
            LoadArgs(0);
            _il.Emit(OpCodes.Newobj, constructor);
            _il.Emit(OpCodes.Stfld, field);
        }

        public void LoadThis()
        {
            LoadArgs(0);
        }

        public void Construct(ConstructorInfo constructor)
        {
            _il.Emit(OpCodes.Newobj, constructor);
        }

        public void Constrained(Type type)
        {
            _il.Emit(OpCodes.Constrained, type);
        }

        public void Throw()
        {
            _il.Emit(OpCodes.Throw);
        }
    }
}