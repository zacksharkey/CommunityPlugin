# CommunityPlugin


## Disclamer: This Plugin was created to use for free at your own risk. Always test this software in your test environment first.

**Tutorials:**

**The Framework Part 1** - https://drive.google.com/file/d/1nBGG5AzUsoUA2_Vo8s2NjiyHiynMvjFm/view?usp=sharing <br>
**Installing the Plugin** - https://drive.google.com/file/d/1y6gBqpOvgqb_0ZW59w5fp3xQhRLrDGdK/view?usp=sharing <br>
**Solutionizing Encompass** - https://drive.google.com/file/d/1b0JPUy5-8RCYW0Wg6seE7SkBV0HcRtZw/view?usp=sharing <br>
**Finding Field Calculations** - https://drive.google.com/file/d/1t8a5TlaGnIpaVwi0lcK6NSAZv5eplTNS/view?usp=sharing

**Introduction:** This Plugin was created as a free way for the community to have a way to modify any part of encompass. This also includes a nice single plugin structure that you can use to further develop this custom to your specifications. Please upload the Settings file in IFB. 

**Settings:** In the CommunitySettings File you will see a variety of properties you can change to suite your needs. Please at the very least update the TestServer to your test server that way you can leave the Permission->AllAccess property blank if you are just testing. 

**Some of the Solutions include:**
- Input Form Automator
- Email Trigger Automator 
- Task Automator using Blocking Queue
- Home Counseling Automator
- Open Input Forms in Separate non Dialog windows
- Open eFolder Documents
- Sort Alerts
- Navigation Buttons
- Disable Services
- Doorbell
- Settings Grid Search
- Loan Folder Loan Count
- Lock Business Rules over multiple instances
- VIP Loans
- Open Loans Read Only
- Savable Pipeline Advanced Searches
- Pipeline Recolor
- Pipeline Loan Reassignment 
- Show FIeldIDs when searching pipeline columns
- Impersonate another user
- Extract all settings from Encompass
- Customizable Loan Side Menu
- AND MORE

## AutomateInputFormSet: 
This is an option to setup a folder inside your InputFormSets Base Folder as 'Persona'. Inside you can create a workflow to apply IFS when a loan opens. You can use all of these prefixes and they apply in this order: 
- User_{UserId}
- LoanType_{LoanTypeValue}_{Persona}
- Milestone_{MilestoneName}_{Persona}
- Persona_{Persona}
- Default

## BackAndForward: 
This is an option you will now see in the Forms/Tools/Services section to easily navigate between forms that have been opened already.
![alt text](Navigation.PNG)

## GridSearch: 
This is an option you will now see in the Settings Menu to search any grid that pops up, self explanitory.
![alt text](GridSearch.PNG)

## HCAutomate: 
This is an option to Automate the closest 10 Home Counseling Providers triggerd by FR0108.


## Impersonate: 
This is an option you will see in the Community Top Menu to impersonate a user as a Super Admin.
![alt text](TopMenu.png)
![alt text](Impersonate.png)

## KickEveryoneOut: 
This is an option to created by Nikolai (EncompDev.com) to kick every non superadmin user out at 4AM and close Encompass.


## OpeneFolderDocument: 
This is an option to set custom field [CX.OPENDOCUMENT] to any placeholder in the efolder documents and the document will open.

## OpenReadOnly: 
This is an option you will now see in the Pipeline Context (right click) menu to Open any Loan Read only.
![alt text](readonly.png)


## SideMenu: 
This is an option to give users a static menu that is always open inside a loan. Everything in this plugin was created to easily expand
upon.

![alt text](SideMenu.PNG)

-  FieldLookup: 
This is a Tool inside the Loan Menu to lookup a field by id, description or value.

-  LoanInformation: 
This is a Tool inside the Loan Menu that you can customize inside of CommunitySettings.json to let different personas see specific fields all the time in a loan file.

- LinksAndResources: 
This is a Tool you can customize inside of CommunitySettings.json to let users click links, open eFolder Documents, etc.

## SettingExtract: 
This is an option you will see in the Community Top Menu to extract all settings to a zip file.
![alt text](TopMenu.png)
![alt text](Settings.png)

## VirtualFields: 
To be Described.



## License
[MIT](https://choosealicense.com/licenses/mit/)
