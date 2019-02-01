SELECT * FROM ro_ticketing.permission;

update ro_ticketing.permission set RoleId='3f3bdf71-adcb-4014-a22f-6ae1c2566837' where RoleId = '5a05e6a2-ddac-4907-9829-fea0da87c064'; /* sysadmin */
update ro_ticketing.permission set RoleId='c8493545-3177-4ac1-984c-a6dcbf50c367' where RoleId = 'd863a12e-6c06-4233-b071-823ddf9e91d2'; /* admin */
update ro_ticketing.permission set RoleId= '7495bbfe-525b-4b23-950a-e0e31ed1311c' where RoleId = '980f64fa-f834-4b74-ae3b-b8e357c96825'; /* driver */
update ro_ticketing.permission set RoleId='104f08d3-a3cc-459f-8ae9-06588d67f347' where RoleId = '980f64fa-f834-4b74-ae3b-b8e357c96825'; /* sales */


/*

980f64fa-f834-4b74-ae3b-b8e357c96825 driver
c4fc1800-00d4-4c31-a532-344fe06466eb sales
1e2bb325-5afc-41f5-90ac-ae19ded0f985 operator
*/