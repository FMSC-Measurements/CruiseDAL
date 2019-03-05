using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.Common
{
    public partial class DDL
    {
        public const string CREATE_TABLE_BIOMASSEQUATION =
            "CREATE TABLE BiomassEquation( " +
                "Species TEXT NOT NULL, " +
                "Product TEXT NOT NULL, " +
                "Component TEXT NOT NULL, " +
                "LiveDead TEXT NOT NULL, " +
                "FIAcode INTEGER NOT NULL, " +
                "Equation TEXT, " +
                "PercentMoisture REAL Default 0.0, " +
                "PercentRemoved REAL Default 0.0, " +
                "MetaData TEXT, " +
                "WeightFactorPrimary REAL Default 0.0, " +
                "WeightFactorSecondary REAL Default 0.0, " +
                "UNIQUE (Species, Product, Component, LiveDead)" +
            ");";
    }
}
