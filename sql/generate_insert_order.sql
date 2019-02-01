SET @insert_stmt = 'INSERT INTO `ro_ticketing`.`order` (`OrderDate`, `RequestDate`, `RequestTime`, `OrderStatusId`, `JobTypeId`, `CustomerId`, `RigLocationId`, `WellID`, `PointOfContactName`, `PointOfContactNumber`, `LoadOrigin`, `OrderDescription`, `IsEnabled`, `IsDeleted`) VALUE (';

SELECT CONCAT(@insert_stmt,'\'', DATE_ADD(creationDate, INTERVAL 4 MONTH), '\',\'', DATE_ADD(requestedDate, INTERVAL 4 MONTH), '\',\'', requestedTime, '\',1,1,1,1,1,\'\',\'\',1,\'', description, '\',1,0);') FROM `ro_ticketing`.`TKS_ORDERS` WHERE creationDate is Not Null;


