The metaData file for each map is formatted as follows,
Delay X Y Z comboNum
this data is parsed into the delay manager, depending on the scene is which file you will manage. 
If you choose to add a new map, a metaData file must be created formatting exactly like this
NOTE: the delay listed in the metadata file is the timing for hitting the note ideally (300). If you wish to get the actual spawn time look at the TargetSpawner class and the logic which determines the value of preeempt 