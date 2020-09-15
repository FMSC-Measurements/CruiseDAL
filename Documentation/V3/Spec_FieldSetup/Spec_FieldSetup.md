
```
classDiagram
    
    
    class StratumFieldSetup{
        + StratumCode
        + FieldName
        + FieldOrder
        - DefaultValue
        - IsHidden
        - IsReadOnly
    }
    
    class SampleGroupFieldSetup{
        + StratumCode
        + SampleGroupCode
        + FieldOrder
        - DefaultValue
        - IsHidden
        - IsReadOnly
    }

    class SubPopulationFieldSetup{
        + StratumCode
        + SampleGroupCode
        + FieldOrder
        - DefaultValue
        - IsHidden
        - IsReadOnly
    }
    
    class TreeDefaultValue {
        + TreeDefaultID
        + PrimaryProduct
        + HiddenPrimary
        + HiddenPrimaryDead
        + CullPrimary
        + CullPrimaryDead
        - ...
    }
    
    class Stratum {
        + StratumCode
    }
    
    class SampleGroup{
        + StratumCode
        + SampleGroupCode
    }
    
    class TreeField{
        + FieldName
        - IsMeasurmentField
    }

    class SubPopulation{
        + StratumCode
        + SampleGroupCode
        + SpeciesCode
    }

    SampleGroup -- Stratum
    StratumFieldSetup -- Stratum
    StratumFieldSetup -- TreeField
    SampleGroupFieldSetup -- SampleGroup
    SampleGroupFieldSetup -- TreeField
    SubPopulationFieldSetup -- SubPopulation
    SubPopulationFieldSetup -- TreeField

```