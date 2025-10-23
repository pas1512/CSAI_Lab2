const string FILE_PATH = "C:\\Users\\DANYIL\\Desktop\\Saved.txt";

/*DataSet rawSet = DataSetTools.PATROL_ROBOT_RAW_SET;*/

/*DataSetTools.Save(FILE_PATH, rawSet);
Console.WriteLine($"Збережено у файл {FILE_PATH}");*/

DataSetTools.Load(FILE_PATH, out var res);
Console.WriteLine(res);