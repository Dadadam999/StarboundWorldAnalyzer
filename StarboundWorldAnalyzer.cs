using System.Text;

public class StarboundWorldAnalyzer
{
    public static void Main()
    {
        string filePath = "example.world"; // Укажите путь к вашему файлу .world

        if( !File.Exists( filePath ) )
        {
            Console.WriteLine( "Файл не найден. Проверьте путь." );
            return;
        }

        Console.WriteLine( $"Анализ файла: {filePath}" );

        try
        {
            using( FileStream fs = new FileStream( filePath, FileMode.Open, FileAccess.Read ) )
            using( BinaryReader reader = new BinaryReader( fs ) )
            {
                // Читаем первые 8 байт: идентификатор формата
                string magic = Encoding.UTF8.GetString( reader.ReadBytes( 8 ) );
                Console.WriteLine( $"Формат файла: {magic}" );

                // Читаем следующую часть, которая может быть номером версии или метаданными
                int version = reader.ReadInt32();
                Console.WriteLine( $"Версия файла: {version}" );

                // Попробуем считать несколько ключей/значений (предположительно)
                Console.WriteLine( "\nПопытка извлечения данных..." );

                while( reader.BaseStream.Position < reader.BaseStream.Length )
                {
                    // Считываем длину ключа
                    int keyLength = reader.ReadInt32();
                    if( keyLength <= 0 || keyLength > 1000 ) break; // Защита от ошибок

                    // Считываем ключ
                    string key = Encoding.UTF8.GetString( reader.ReadBytes( keyLength ) );
                    Console.WriteLine( $"Ключ: {key}" );

                    // Считываем длину значения
                    int valueLength = reader.ReadInt32();
                    if( valueLength <= 0 || valueLength > 10000 ) break; // Защита от ошибок

                    // Считываем значение (ограничимся строками)
                    string value = Encoding.UTF8.GetString( reader.ReadBytes( valueLength ) );
                    Console.WriteLine( $"  Значение: {value}" );
                }
            }
        }
        catch( Exception ex )
        {
            Console.WriteLine( $"Ошибка при чтении файла: {ex.Message}" );
        }
    }
}
