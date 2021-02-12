# CommunityPlugin


## Disclamer: This Plugin was created to use for free at your own risk. Always test this software in your test environment first.

**Tutorials:**

**The Framework Part 1** - https://youtu.be/9juibPIQZUg <br>
**Installing the Plugin** - https://youtu.be/sA2pMGhBxeM <br>
**Solutionizing Encompass** - https://youtu.be/vZD6_ufyQ1Y <br>
**Finding Field Calculations** - https://youtu.be/8T9deHoDOjw <br>
**Debugging your plugin** - https://youtu.be/4k5jkDpJVz0 <br>

**Introduction:** This Plugin was built in an effort to help the community rapidly create ideas in Encompass. 

**Settings:** The new Plugin Management Screen is where you will setup all of your plugin access rights
<img src="https://media-exp1.licdn.com/dms/image/C4E22AQHUm8v-2GTklQ/feedshare-shrink_2048_1536-alternative/0?e=1602115200&v=beta&t=d5buVutAPDerz2xOmSyBuL4okXsLMjLJxDPw00_jIUU"> <br>

**Updates:**</br>
1.24.2021 - New **Loan Folder Move Tool** </br>
1.24.2021 - Added ability to Parse Expressions</br>
1.25.2021 - Added **Batch Update Tool**</br>
2.11.2021 - Added ability to Export GSE services including 3.4 

</br></br>

**SOME OF THE SOLUTIONS INCLUDE:**<br>
**Input Form Automator :** Automate Applying Input Form sets based on persona, milestone, loan type, user, etc<br>
**Email Trigger Automator :** Use a client to run automated emails.<br>
**Task Automator using Blocking Queue :** Task Automation that enables rapid creation of automation.<br>
**Home Counseling Automator :** Automate first 10 Home Counselors on FE Zip code change.<br>
**Open Input Forms in Separate non Dialog windows :** Open Encompass Input Form inside Windows form non dialog. <br>
**Open eFolder Documents :** Set custom field CX.OPENDOCUMENT to the name of a Document bucket to open the Document directly.<br>
**Sort Alerts :** Sorts Alerts on loan open alphabetically.<br>
**Navigation Buttons :** Navigate back and forth between previously clicked input forms <br>
**Disable Services :** Disable all or some services.<br>
**Doorbell :** Request access to user who locked loan and receive notification when they are out of the loan.<br>
**Settings Grid Search :** Adds a Textbox to search any Setting Tree Node with a GridView.<br>
**Loan Folder Loan Count :** Counts the number of loans per folder in the Loan Folder Setting node.<br>
**Lock Business Rules over multiple instances :** POC to lock business rules over instances.<br>
**VIP Loans :** Remove all loans from pipeline if user isn't a member of VIP User Group.<br>
**Open Loans Read Only :** Add option to Pipeline Context Menu to open a loan in read only mode.<br>
**Savable Pipeline Advanced Searches :** Save advanced search criteria per pipeline view for User Customizable Pipeline Views. <br>
**Pipeline Recolor :** Color pipeline based on rules.<br>
**Pipeline Loan Reassignment :** Moved the Loan Reassignment tool from Settings to the Pipeline Context Menu so you can select applicable loans in the Pipeline.<br>
**Show FieldIDs when searching pipeline columns :** When customizing Pipeline View Columns, add a column that shows the corresponding FieldIDs.<br>
**Impersonate another user :** Impersonate another user lower on the hierarchy level and return to your rights.<br>
**Extract all settings from Encompass :** Extract settings that aren't exportable such as Password Management for services.<br>
**Customizable Loan Side Menu :** Static collapsable menu that can hold any user control, dynamic load based on persona.<br>
**AND MORE**<br><br>

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
