using System;

namespace Enigma.Test.TestDb
{
    public static class RandomCars
    {
        public static Car AK9777 = new Car {
            RegistrationNumber = "AK9777",
            Nationality = Nationality.Sweden,
            EstimatedValue = 110000,
            EstimatedAt = new DateTime(2011, 7, 1),
            Model = new CarModel {
                Brand = CarBrand.Toyota,
                Name = "Yaris",
                Year = 2012
            },
            Engine = new CarEngine {
                HorsePower = 127,
                CylinderCount = 8
            },
            Compartments =
                {
                    new Compartment {
                        Description = "Front left",
                        SquareMeters = 0.2d
                    },
                    new Compartment {
                        Description = "Front right",
                        SquareMeters = 0.2d
                    },
                    new Compartment {
                        Description = "Rear",
                        SquareMeters = 2.0d
                    }
                }
        };

        public static Car NDN100 = new Car {
            RegistrationNumber = "NDN100",
            Nationality = Nationality.Sweden,
            EstimatedValue = 6000,
            EstimatedAt = new DateTime(2006, 9, 1),
            Model = new CarModel {
                Brand = CarBrand.Ford,
                Name = "Fiesta",
                Year = 1988
            },
            Engine = new CarEngine {
                HorsePower = 77,
                CylinderCount = 8
            },
            Compartments =
                {
                    new Compartment {
                        Description = "Right front",
                        SquareMeters = 0.2d
                    },
                    new Compartment {
                        Description = "Rear",
                        SquareMeters = 1.8d
                    }
                }
        };

        public static Car MDS800 = new Car {
            RegistrationNumber = "MDS800",
            Nationality = Nationality.Sweden,
            EstimatedValue = 30000,
            EstimatedAt = new DateTime(2011, 6, 1),
            Model = new CarModel {
                Brand = CarBrand.Audi,
                Name = "A3",
                Year = 2001
            },
            Engine = new CarEngine {
                HorsePower = 211,
                CylinderCount = 8
            },
            Compartments =
                {
                    new Compartment {
                        Description = "Right front",
                        SquareMeters = 0.22d
                    },
                    new Compartment {
                        Description = "Rear",
                        SquareMeters = 2.5d
                    }
                }
        };
    }
}
