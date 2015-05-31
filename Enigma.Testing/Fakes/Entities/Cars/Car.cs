using System;
using System.Collections.Generic;

namespace Enigma.Testing.Fakes.Entities.Cars
{
    public class Car
    {

        public Car()
        {
            Compartments = new List<Compartment>();
        }

        public string RegistrationNumber { get; set; }
        public Nationality Nationality { get; set; }
        public int EstimatedValue { get; set; }
        public DateTime EstimatedAt { get; set; }

        public CarEngine Engine { get; set; }
        public CarModel Model { get; set; }
        public ICollection<Compartment> Compartments { get; set; }
    }
}
