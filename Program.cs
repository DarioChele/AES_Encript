using System;
using System.Security.Cryptography;
using System.Text;

class Program {
    static Dictionary<string, (string es, string en)> texts = new Dictionary<string, (string, string)> {
        { "menu", ("Menú:", "Menu:") },
        { "opcion1", ("1. Generar Key", "1. Generate Key") },
        { "opcion2", ("2. Encriptar texto", "2. Encrypt text") },
        { "opcion3", ("3. Desencriptar texto", "3. Decrypt text") },
        { "opcion4", ("4. Salir", "4. Exit") },
        { "seleccionar", ("Selecciona una opción: ", "Select an option: ") },
        { "secure_key", ("Ingrese la llave de seguridad: ", "Enter the secure Key: ") },
        { "ingresar_texto", ("Ingrese el texto a encriptar: ", "Enter the text to encrypt: ") },
        { "ingresar_texto_encriptado", ("Ingrese el texto a desencriptar: ", "Enter the text to decrypt: ") },
        { "KeyGenerada", ("Llave generada: ", "Generated key: ") },
        { "texto_encriptado", ("Texto encriptado : ", "Encrypted Text: ") },
        { "texto_desencriptado", ("Texto desencriptado : ", "Decrypted Text: ") },
        { "closing", ("Saliendo... ", "Closing... ") },
        { "errOpt", ("Error: La opcion seleccionada no es válida.", "Error: The chosen option is not valid.") },
        { "errKey", ("Error: La key proporcionada no es válida o el texto no puede ser desencriptado.", "Error: The provided key is not valid or the text cannot be decrypted.") }
    };
    static string getText(string clave) {
        return lang == "es" ? texts[clave].es : texts[clave].en;
    }

    static string lang = "";
    static void Main(){
        Console.OutputEncoding = Encoding.UTF8;
        var flag = true;
        while (flag ) {
            flag = false;            
            Console.Write("Sel idioma/language 1 for Eng - 2 Espanol: ");
            lang = Console.ReadLine().ToLower().Trim();
            if (!string.IsNullOrWhiteSpace(lang)){
                switch (lang){
                    case "1":
                        lang = "en";
                        break;
                    case "2":
                        lang = "es";
                        break;
                    default:
                        flag = true;
                        Console.WriteLine("Invalid option. Please type '1' for English or '2' for Spanish. | Idioma no válido. Por favor, ingrese 1 para inglés o 2 para español");
                        continue;
                }
            }
        }
        Console.Clear();
        while (true)
        {
            Console.WriteLine(Environment.NewLine + getText("menu"));
            Console.WriteLine(getText("opcion1"));
            Console.WriteLine(getText("opcion2"));
            Console.WriteLine(getText("opcion3"));
            Console.WriteLine(getText("opcion4"));
            Console.Write(getText("seleccionar"));
            string opcion = Console.ReadLine();

            if (opcion == "1")
            {
                string keyGenerada = GenerarKey();
                Console.WriteLine(getText("KeyGenerada") + keyGenerada);
            }
            else if (opcion == "2") {
                Console.Write(getText("secure_key"));
                string key = Console.ReadLine();
                Console.Write(getText("ingresar_texto"));
                string texto = Console.ReadLine();
                string textoEncriptado = Encriptar(texto, key);
                Console.WriteLine(getText("texto_encriptado") + textoEncriptado);
            } else if (opcion == "3"){
                Console.Write(getText("secure_key"));
                string key = Console.ReadLine();
                Console.Write(getText("ingresar_texto_encriptado"));
                string textoEncriptado = Console.ReadLine();
                string textoDesencriptado = Desencriptar(textoEncriptado, key);
                Console.WriteLine(getText("texto_desencriptado") + textoDesencriptado);
            }
            else if (opcion == "4")
            {
                Console.WriteLine(getText("closing"));
                break;
            }
            else
            {
                Console.WriteLine(getText("errOpt"));
            }
        }
    }

    static string GenerarKey() {
        using (Aes aes = Aes.Create()) {
            return Convert.ToBase64String(aes.Key);
        }
    }

    static string Encriptar(string texto, string key) {
        using (Aes aes = Aes.Create()){
            aes.Key = Convert.FromBase64String(key);
            aes.GenerateIV();
            using (ICryptoTransform encryptor = aes.CreateEncryptor()){
                byte[] datos = Encoding.UTF8.GetBytes(texto);
                byte[] cifrado = encryptor.TransformFinalBlock(datos, 0, datos.Length);
                return Convert.ToBase64String(aes.IV) + ":" + Convert.ToBase64String(cifrado);
            }
        }
    }

    static string Desencriptar(string textoEncriptado, string key){
        try {
            string[] partes = textoEncriptado.Split(':');
            byte[] iv = Convert.FromBase64String(partes[0]);
            byte[] cifrado = Convert.FromBase64String(partes[1]);

            using (Aes aes = Aes.Create()) {
                aes.Key = Convert.FromBase64String(key);
                aes.IV = iv;
                using (ICryptoTransform decryptor = aes.CreateDecryptor()) {
                    byte[] datosDescifrados = decryptor.TransformFinalBlock(cifrado, 0, cifrado.Length);
                    return Encoding.UTF8.GetString(datosDescifrados);
                }
            }
        } catch {
            return getText("errKey");
        }
    }
}
