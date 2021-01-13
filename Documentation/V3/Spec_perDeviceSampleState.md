This document is a work in progress
# Per Device Sample State

To help facilitate the portability of cruise data across devices and allow merging cruise data between files, while maintaining the integrity of the sample selection process. The cruise file will contain separate records for sampling state for each device used in the cruise.  

## Opening cruise on new device
When opening a cruise on a new device. If the cruise has a sample state from another device, the user will be prompted to continue cruising from a preexisting sample state or start with a fresh sample state

If the user chooses to start with a preexisting sample state. A copy of the preexisting sample state will be made and associated with the new device. 
 
## Portability of cruise files
The reason for allowing users to either start with a fresh sample state or continue with a existing sample state is to allow for the portability of the cruise file across devices. 
We envision two scenarios
 - new cruiser joins the cruise bringing an additional device to cruise along side other cruisers. They will chose to start with a fresh sample state because the original cruiser will continue cruising. 
 - a cruise is being carried out primary on one device but for some reason (someone goes on vacation, device has issues) another device needs to be used in place of an existing device

## Allowing for merging cruise data
When multiple copies of a cruise are merged together, we need to be able to keep the sample state from each device.

The merging process will take multiple copies of a cruise file and then output a merged copy. 
To maintain the integrity of the sampling state from all the original cruise files. We use a separate sample state record for each device. Sample states will also be time stamped so that if a sample state for a device exists in multiple files only the latest sample state will be merge into the output file. 


