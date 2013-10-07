namespace Enigma.Modelling
{
    public class ModelBuilder
    {

        private readonly Model _model;

        public ModelBuilder()
        {
            _model = new Model();
        }

        public ModelBuilder(Model model)
        {
            _model = model;
        }

        public EntityBuilder<T> Entity<T>()
        {
            return new EntityBuilder<T>(_model.Entity<T>());
        }

    }
}
