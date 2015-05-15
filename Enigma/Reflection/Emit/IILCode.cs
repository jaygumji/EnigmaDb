namespace Enigma.Reflection.Emit
{
    public interface IILCode
    {

        void Generate(ILExpressed il);

    }

    public interface IILCodeParameter
    {

        void Load(ILExpressed il);
        void LoadAddress(ILExpressed il);

    }

}
