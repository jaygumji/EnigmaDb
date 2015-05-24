namespace Enigma.Reflection.Emit
{
    public delegate void ILGenerationHandler();
    public delegate void ILGenerationMethodHandler(ILExpressed il);

    public delegate void ILGenerationHandler<in T>(T value) where T : ILCodeParameter;
    public delegate void ILGenerationMethodHandler<in T>(ILExpressed il, T value) where T : ILCodeParameter;
}