using System;

namespace Enigma.Test.TestDb
{
    public static class TheACars
    {
        public static Car AAA001 = new Car {
            RegistrationNumber = "AAA001",
            Nationality = Nationality.Denmark,
            EstimatedValue = 40000,
            EstimatedAt = new DateTime(1980, 3, 3),
            Model = new CarModel {
                Brand = CarBrand.Mitsubishi,
                Name = "CAR1",
                Year = 1980
            },
            Engine = new CarEngine {
                HorsePower = 50,
                CylinderCount = 8
            },
            Compartments =
                {
                    new Compartment {
                        Description = "Right front",
                        SquareMeters = 0.20
                    },
                    new Compartment {
                        Description = "Rear",
                        SquareMeters = 1.0d
                    }
                }
        };

        public static Car AAA002 = new Car {
            RegistrationNumber = "AAA002",
            Nationality = Nationality.Finland,
            EstimatedValue = 50000,
            EstimatedAt = new DateTime(1981, 3, 3),
            Model = new CarModel {
                Brand = CarBrand.Nissan,
                Name = "CAR2",
                Year = 1981
            },
            Engine = new CarEngine {
                HorsePower = 60,
                CylinderCount = 8
            },
            Compartments =
                {
                    new Compartment {
                        Description = "Right front",
                        SquareMeters = 0.21
                    },
                    new Compartment {
                        Description = "Rear",
                        SquareMeters = 1.2d
                    }
                }
        };

        public static Car AAA003 = new Car {
            RegistrationNumber = "AAA003",
            Nationality = Nationality.Finland,
            EstimatedValue = 60000,
            EstimatedAt = new DateTime(1982, 3, 3),
            Model = new CarModel {
                Brand = CarBrand.Nissan,
                Name = "CAR3",
                Year = 1982
            },
            Engine = new CarEngine {
                HorsePower = 70,
                CylinderCount = 8
            },
            Compartments =
                {
                    new Compartment {
                        Description = "Right front",
                        SquareMeters = 0.22
                    },
                    new Compartment {
                        Description = "Rear",
                        SquareMeters = 1.4d
                    }
                }
        };

        public static Car AAA004 = new Car {
            RegistrationNumber = "AAA004",
            Nationality = Nationality.Iceland,
            EstimatedValue = 770000,
            EstimatedAt = new DateTime(2001, 3, 3),
            Model = new CarModel {
                Brand = CarBrand.AstonMartin,
                Name = "CAR4",
                Year = 2001
            },
            Engine = new CarEngine {
                HorsePower = 370,
                CylinderCount = 8
            },
            Compartments =
                {
                    new Compartment {
                        Description = "Right front",
                        SquareMeters = 0.42
                    },
                    new Compartment {
                        Description = "Rear",
                        SquareMeters = 1.8d
                    }
                }
        };

        public static Car AAA005 = new Car {
            RegistrationNumber = "AAA005",
            Nationality = Nationality.Norway,
            EstimatedValue = 70000,
            EstimatedAt = new DateTime(1983, 3, 3),
            Model = new CarModel {
                Brand = CarBrand.Ford,
                Name = "CAR5",
                Year = 1983
            },
            Engine = new CarEngine {
                HorsePower = 80,
                CylinderCount = 8
            },
            Compartments =
                {
                    new Compartment {
                        Description = "Right front",
                        SquareMeters = 0.23
                    },
                    new Compartment {
                        Description = "Rear",
                        SquareMeters = 1.6d
                    }
                }
        };

        public static Car AAA006 = new Car {
            RegistrationNumber = "AAA006",
            Nationality = Nationality.Norway,
            EstimatedValue = 80000,
            EstimatedAt = new DateTime(1984, 3, 3),
            Model = new CarModel {
                Brand = CarBrand.Ford,
                Name = "CAR6",
                Year = 1984
            },
            Engine = new CarEngine {
                HorsePower = 90,
                CylinderCount = 8
            },
            Compartments =
                {
                    new Compartment {
                        Description = "Right front",
                        SquareMeters = 0.24
                    },
                    new Compartment {
                        Description = "Rear",
                        SquareMeters = 1.8d
                    }
                }
        };

        public static Car AAA007 = new Car {
            RegistrationNumber = "AAA007",
            Nationality = Nationality.Denmark,
            EstimatedValue = 120000,
            EstimatedAt = new DateTime(1988, 3, 3),
            Model = new CarModel {
                Brand = CarBrand.Mitsubishi,
                Name = "CAR7",
                Year = 1987
            },
            Engine = new CarEngine {
                HorsePower = 85,
                CylinderCount = 6
            },
            Compartments =
                {
                    new Compartment {
                        Description = "Right front",
                        SquareMeters = 0.20
                    },
                    new Compartment {
                        Description = "Rear",
                        SquareMeters = 1.7d
                    }
                }
        };

        public static Car AAA008 = new Car {
            RegistrationNumber = "AAA008",
            Nationality = Nationality.Finland,
            EstimatedValue = 130000,
            EstimatedAt = new DateTime(1991, 3, 3),
            Model = new CarModel {
                Brand = CarBrand.Saab,
                Name = "CAR8",
                Year = 1989
            },
            Engine = new CarEngine {
                HorsePower = 95,
                CylinderCount = 6
            },
            Compartments =
                {
                    new Compartment {
                        Description = "Right front",
                        SquareMeters = 0.20
                    },
                    new Compartment {
                        Description = "Rear",
                        SquareMeters = 1.7d
                    }
                }
        };

        public static Car AAA009 = new Car {
            RegistrationNumber = "AAA009",
            Nationality = Nationality.Denmark,
            EstimatedValue = 210000,
            EstimatedAt = new DateTime(2002, 3, 3),
            Model = new CarModel {
                Brand = CarBrand.Renault,
                Name = "CAR9",
                Year = 2000
            },
            Engine = new CarEngine {
                HorsePower = 130,
                CylinderCount = 8
            },
            Compartments =
                {
                    new Compartment {
                        Description = "Right front",
                        SquareMeters = 0.16
                    },
                    new Compartment {
                        Description = "Rear",
                        SquareMeters = 4d
                    }
                }
        };

        public static Car AAA010 = new Car {
            RegistrationNumber = "AAA010",
            Nationality = Nationality.Iceland,
            EstimatedValue = 190000,
            EstimatedAt = new DateTime(1997, 6, 1),
            Model = new CarModel {
                Brand = CarBrand.Toyota,
                Name = "CAR10",
                Year = 1993
            },
            Engine = new CarEngine {
                HorsePower = 103,
                CylinderCount = 6
            },
            Compartments =
                {
                    new Compartment {
                        Description = "Right front",
                        SquareMeters = 0.20
                    },
                    new Compartment {
                        Description = "Rear",
                        SquareMeters = 1.6d
                    }
                }
        };

    }
}