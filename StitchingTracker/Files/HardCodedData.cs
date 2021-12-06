using System.Collections.Generic;

namespace StitchingTracker.Files
{
    public class HardCodedData
    {
        public static Dictionary<string, List<string>> Accounts = new Dictionary<string, List<string>>()
        {
            {
                "EMB Acc*", new List<string>()
                {
                    "Candle", "Hi-Speed", "Ikram Traders"
                }
            },

            {
                "EMB Fabric*", new List<string>()
                {
                    "Hameed", "Malik SB", "Paindu Col", "Shafaqat", "Rasheed Sb",
                    "Khadija Arts"
                }
            },

            {
                "Nazy Zari*", new List<string>()
                {
                    "Anees"
                }
            },

            {
                "Nazy Acc*", new List<string>()
                {
                    ""
                }
            },

            {
                "Nazy Order*", new List<string>()
                {
                    "Nazy-"
                }
            },

            {
                "Nazy Fabric*", new List<string>()
                {
                    ""
                }
            },

            {
                "ShahzaibEMB", new List<string>()
                {
                    "Raw Material", "Fabric"
                }
            },

            {
                "Nazy", new List<string>()
                {
                    "Raw Material", "Finished"
                }
            },

            {
                "Outlets", new List<string>()
                {
                    "Awrish", "Online"
                }
            }
        };

        public static Dictionary<string, List<string>> UnitTypes = new Dictionary<string, List<string>>()
        {
            {
                // This address is also specified in AddNewUnit window.
                // Don't Change First entry's index not the first one's first value's index
                "ThreePiece", new List<string>()
                {
                    "OrderNum", "Size", "Color", "Note"
                }
            },

            {
                "Fabric", new List<string>()
                {
                   "Kind", "Color", "PRate", "Note"
                }
            },

            {
                "Zari", new List<string>()
                {
                    "Color", "Note"
                }
            },

            {
                "Thread", new List<string>()
                {
                    "Shade", "Note"
                }
            },
        };

        public static Dictionary<string, List<Dictionary<string, List<string>>>> Suggestions = new Dictionary<string, List<Dictionary<string, List<string>>>>()
        {
            {
                "ThreePiece", new List<Dictionary<string, List<string>>>()
                {
                    {
                        new Dictionary<string, List<string>>()
                        {
                            {
                                "Article", new List<string>()
                                {
                                    "NZ"
                                }
                            },

                            {
                                "Size", new List<string>()
                                {
                                    "Small", "Medium", "Large"
                                }
                            },

                            {
                                "Color", new List<string>()
                                {
                                    "Red", "White", "Black", "Skin"
                                }
                            },
                        }
                    }
                }
            },

            {
                "Thread", new List<Dictionary<string, List<string>>>()
                {
                    {
                        new Dictionary<string, List<string>>()
                        {
                            {
                                "Shade", new List<string>()
                                {
                                    "1278", "1321", "1000", "1084"
                                }
                            },
                        }
                    }
                }
            },

            {
                "Fabric", new List<Dictionary<string, List<string>>>()
                {
                    {
                        new Dictionary<string, List<string>>()
                        {
                            {
                                "Kind", new List<string>()
                                {
                                    "Shafoon", "Cotton", "PC Cotton"
                                }
                            },

                            {
                                "Color", new List<string>()
                                {
                                    "Red", "Pista", "Skin", "White", "Black"
                                }
                            },
                        }
                    }
                }
            }
        };
    }
}
