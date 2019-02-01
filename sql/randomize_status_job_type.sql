use  `ro_ticketing`;

set @min = 1;
set @max = 4;
UPDATE `order` set OrderStatusId = FLOOR((RAND() * (@max-@min+1))+@min);


set @min = 6;
set @max = 11;
UPDATE `order` set JobTypeId = FLOOR((RAND() * (@max-@min+1))+@min)

