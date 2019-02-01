SET FOREIGN_KEY_CHECKS = 0;

truncate table ro_ticketing.orderstatus;

INSERT INTO `ro_ticketing`.`orderstatus`(`Id`, `Name`, `IsEnabled`, `IsDeleted`, `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`) VALUES (0, 'PRETICKET', b'1', b'0', '2018-10-22 09:38:20', '', '2018-10-22 09:52:58', '');

INSERT INTO `ro_ticketing`.`orderstatus`(`Id`, `Name`, `IsEnabled`, `IsDeleted`, `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`) VALUES (1, 'OPEN', b'1', b'0', '2018-10-22 09:37:17', '', '2018-10-22 09:50:44', '');

INSERT INTO `ro_ticketing`.`orderstatus`(`Id`, `Name`, `IsEnabled`, `IsDeleted`, `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`) VALUES (2, 'ASSIGNED', b'1', b'0', '2018-10-22 09:37:36', '', '2018-10-22 09:51:44', '');

INSERT INTO `ro_ticketing`.`orderstatus`(`Id`, `Name`, `IsEnabled`, `IsDeleted`, `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`) VALUES (3, 'LOADED', b'1', b'1', '2018-10-22 09:37:52', '', '2018-10-22 09:52:15', '');

INSERT INTO `ro_ticketing`.`orderstatus`(`Id`, `Name`, `IsEnabled`, `IsDeleted`, `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`) VALUES (4, 'DELIVERED', b'1', b'0', '2018-10-22 09:38:03', '', '2018-10-22 09:52:40', '');

INSERT INTO `ro_ticketing`.`orderstatus`(`Id`, `Name`, `IsEnabled`, `IsDeleted`, `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`) VALUES (5, 'VOIDED', b'1', b'0', '2018-10-22 09:38:20', '', '2018-10-22 09:52:58', '');

SET FOREIGN_KEY_CHECKS = 1;