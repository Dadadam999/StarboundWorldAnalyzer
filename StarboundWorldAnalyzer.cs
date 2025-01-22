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

                // Читаем следующую часть
                int version = reader.ReadInt32();
                Console.WriteLine( $"Версия файла: {version}" );

                Console.WriteLine( "\nПопытка извлечения данных..." );

                // Логирование содержимого файла
                LogFileContent( filePath, "file_dump.log" );

                // Анализируем оставшуюся структуру
                while( reader.BaseStream.Position < reader.BaseStream.Length )
                {
                    // Попробуем считать данные как ключи и значения
                    try
                    {
                        int keyLength = reader.ReadInt32();
                        if( keyLength <= 0 || keyLength > 1000 ) break;

                        string key = Encoding.UTF8.GetString( reader.ReadBytes( keyLength ) );
                        Console.WriteLine( $"Ключ: {key}" );

                        int valueLength = reader.ReadInt32();
                        if( valueLength <= 0 || valueLength > 10000 ) break;

                        string value = Encoding.UTF8.GetString( reader.ReadBytes( valueLength ) );
                        Console.WriteLine( $"  Значение: {value}" );
                    }
                    catch
                    {
                        Console.WriteLine( "Не удалось извлечь данные. Файл имеет другую структуру." );
                        break;
                    }
                }
            }
        }
        catch( Exception ex )
        {
            Console.WriteLine( $"Ошибка при чтении файла: {ex.Message}" );
        }
    }

    // Логирование файла в HEX-формате
    private static void LogFileContent( string filePath, string logPath )
    {
        using( FileStream fs = new FileStream( filePath, FileMode.Open, FileAccess.Read ) )
        using( StreamWriter log = new StreamWriter( logPath ) )
        {
            byte[] buffer = new byte[16];
            int bytesRead;
            int offset = 0;

            log.WriteLine( $"Hex dump of {filePath}:" );

            while( ( bytesRead = fs.Read( buffer, 0, buffer.Length ) ) > 0 )
            {
                // Вывод адреса
                log.Write( $"{offset:X8}: " );
                offset += bytesRead;

                // Вывод байтов в HEX
                for( int i = 0; i < bytesRead; i++ )
                {
                    log.Write( $"{buffer[i]:X2} " );
                }

                // Заполняем оставшиеся байты пробелами
                for( int i = bytesRead; i < buffer.Length; i++ )
                {
                    log.Write( "   " );
                }

                // Вывод ASCII-представления
                log.Write( " | " );
                for( int i = 0; i < bytesRead; i++ )
                {
                    char c = (char) buffer[i];
                    log.Write( char.IsControl( c ) ? '.' : c );
                }

                log.WriteLine();
            }
        }

        Console.WriteLine( $"Содержимое файла сохранено в {logPath}" );
    }
}
