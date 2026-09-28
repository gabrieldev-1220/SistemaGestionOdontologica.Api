using System.Security.Cryptography;

Console.Title = "JWT Key Generator";

Console.WriteLine(new string('=',60));
Console.WriteLine("\tJWT KEY GENERATOR APP");
Console.WriteLine(new string('=',60));
Console.WriteLine();

Console.Write("Cantidad de bytes para la clave [64]: ");

string? input = Console.ReadLine();

int keySize = 64;

if (!string.IsNullOrWhiteSpace(input))
{
    if (!int.TryParse(input, out keySize))
    {
        Console.WriteLine();
        Console.WriteLine("Valor inválido. Se utilizarán 64 bytes.");
        keySize = 64;
    }
}

if (keySize < 32)
{
    Console.WriteLine();
    Console.WriteLine("La llave debe tener al menos 32 bytes.");
    Console.WriteLine("Se utilizarán 64 bytes.");
    keySize = 64;
}

Console.WriteLine();
Console.WriteLine($"Generador clave de {keySize} bytes...");
Console.WriteLine();

byte[] keyBytes = RandomNumberGenerator.GetBytes(keySize);

string jwtKey = Convert.ToBase64String(keyBytes);

Console.WriteLine(new string('=', 60));
Console.WriteLine($"CLAVE JWT GENERADA: {jwtKey}");
Console.WriteLine(new string('=', 60));
Console.WriteLine();

Console.WriteLine($"Bytes generados: {keySize}");
Console.WriteLine($"Caracteres Base64: {jwtKey.Length}");
Console.WriteLine();

Console.WriteLine("Copiar esta clave y guardarla de manera segura.");
Console.WriteLine();

Console.WriteLine("Presionar ENTER para finalizar...");
Console.ReadLine();