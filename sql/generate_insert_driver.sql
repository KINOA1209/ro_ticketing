
SELECT CONCAT('INSERT INTO `ro_ticketing`.`driver` (`Name`, `Code`, `Phone`, `Position`, `IsEnabled`, `IsDeleted`) VALUES  ( ''', COALESCE(name,''), ''',''', COALESCE(code,''), ''',''', COALESCE(phone,''), ''',''', COALESCE(position,''), ''', 1, 0 );')
FROM ro_ticketing.DRIVERS;
 
