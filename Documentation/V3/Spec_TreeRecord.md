## Tree number unique per plot-stratum
Although typically tree number should be unique per plot, in a recon cruise, users may want to allow duplicate tree numbers within a plot so long as the stratum is different.
For this reason we can only enforce uniqueness to the plot-stratum level. 
To enforce this we can use a partial index that ensures trees unique using cruise, unit, plot, stratum WHERE plotnumber is not null.

A solution to this would be to use a separate column for recon plots. 



