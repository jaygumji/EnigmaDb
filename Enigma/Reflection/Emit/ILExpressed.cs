using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Enigma.Reflection.Emit
{
    public sealed class ILExpressed
    {
        private readonly ILGenerator _il;
        private readonly TypeCache _typeCache;
        private readonly ILCodeSnippets _snippets;
        private readonly ILCodeVariableGenerator _var;

        public ILExpressed(ILGenerator il) : this(il, new TypeCache())
        {
        }

        public ILExpressed(ILGenerator il, TypeCache typeCache)
        {
            _il = il;
            _typeCache = typeCache;
            _snippets = new ILCodeSnippets(this);
            _var = new ILCodeVariableGenerator(this);
        }

        public ILGenerator Gen { get { return _il; } }
        public TypeCache TypeCache { get { return _typeCache; } }
        public ILCodeSnippets Snippets { get { return _snippets; } }
        public ILCodeVariableGenerator Var { get { return _var; } }

        public void LoadValue(int value)
        {
            OpCode opCode;
            if (OpCodesLookups.LoadInt32.TryGetValue(value, out opCode)) {
                _il.Emit(opCode);
                return;
            }

            _il.Emit(OpCodes.Ldc_I4_S, value);
        }

        public void LoadValue(uint value)
        {
            LoadValue((int)value);
        }

        public void LoadValue(string value)
        {
            _il.Emit(OpCodes.Ldstr, value);
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

        public LocalILCodeVariable DeclareLocal(string name, Type type)
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

        public void TransferIfNull(ILCodeVariable reference, Label label)
        {
            _var.Load(reference);
            LoadNull();
            CompareEquals();
            TransferLongIfTrue(label);
        }

        public void TransferIfNotNull(ILCodeVariable reference, Label label)
        {
            _var.Load(reference);
            LoadNull();
            CompareEquals();
            TransferLongIfFalse(label);
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
            LoadThis();
            _il.Emit(OpCodes.Newobj, constructor);
            _il.Emit(OpCodes.Stfld, field);
        }

        public void LoadThis()
        {
            _il.Emit(OpCodes.Ldarg_0);
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

        public void Generate(IILCode code)
        {
            code.Generate(this);
        }

        public void Negate()
        {
            LoadValue(0);
            CompareEquals();
        }

    }
}