use ro_ticketing;


ALTER TABLE jobtype ADD COLUMN CreatedAt datetime NOT NULL DEFAULT '0001-01-01 00:00:00';
ALTER TABLE jobtype ADD COLUMN CreatedBy tinytext;
ALTER TABLE jobtype ADD COLUMN UpdatedAt datetime NOT NULL DEFAULT '0001-01-01 00:00:00';
ALTER TABLE jobtype ADD COLUMN UpdatedBy tinytext;


ALTER TABLE `ro_ticketing`.`market` 
ADD COLUMN `IsEnabled`  BIT(1) NOT NULL DEFAULT 1 ,
ADD COLUMN `IsDeleted` BIT(1) NOT NULL DEFAULT 0 ;

ALTER TABLE `ro_ticketing`.`market` 
CHANGE COLUMN `IsEnabled` `IsEnabled` BIT(1) NOT NULL DEFAULT 1 ,
CHANGE COLUMN `IsDeleted` `IsDeleted` BIT(1) NOT NULL DEFAULT 0 ;


