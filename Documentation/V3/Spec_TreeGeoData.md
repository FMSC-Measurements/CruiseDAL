 # Tree geo-data

Users want to be able to collect the location of trees to help 
 - relocate measure trees for checking measurements
 - relocate insurance trees
 - collect location for boundary trees
 - collect location of compliance/administrative/leave trees

 ## Side shots 
Many times accurate gps location can not be taken close to the tree or it is not practical for the operator to go to the tree. 
The solution is to do a side shot, where a gps location is taken away from the tree and a azimuth and distance are measured to get the actual location of the tree. 

 ## Quality of measurement meta data
Generally tree locations just need to be accurate enough to allow someone navigate back to the general area of the the tree. Side shot values might not be done with accuracy as a goal. 
Although we can automatically determine accuracy of gps locations, we have no way to determine accuracy of  azimuth and/or distance data entered by the user. 
We want to allow users to indicate if measurements are estimates

 ## Location Fields
 - Latatude
 - Longatude
 - SS_Latatude
 - SS_Longatude
 - Asmuth
 - Distance
 - IsEstimate
 

