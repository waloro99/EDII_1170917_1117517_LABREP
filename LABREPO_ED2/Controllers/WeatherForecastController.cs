using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LABREPO_ED2.ClassLab1;
using LABREPO_ED2.Repository;
using LABREPO_ED2.ClassLab3_4;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using LABREPO_ED2.ClassLab5;

namespace LABREPO_ED2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        //public WeatherForecastController(ILogger<WeatherForecastController> logger)
        //{
        //    _logger = logger;
        //}

        public static IWebHostEnvironment _environment;

        public WeatherForecastController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        //object file
        public class FileUploadAPI
        {
            public IFormFile files { get; set; } //name use in postman --> files

        }

        //------------------------------ ENDPOINTS LAB 1 -----------------------------------------------
        private static readonly ISodasDataBase SDatabase = new SodasDataBase();

        // localhost:51626/weatherforecast/GetWithParam/?SearchSoda=""
        [HttpGet("GetWithParam", Name = "GetSoda")]
        /*[Route("weatherforecast/Sodas/")]*/
        public IEnumerable<Soda> Get(string SearchSoda)
        {
            if (SearchSoda == null)
            {
                return SDatabase.GetSodas();
            }
            else
            {
                List<Soda> SodaOrden = new List<Soda>(); //new list
                SodaOrden.Add(SDatabase.ReturnMySoda(SearchSoda));
                return SodaOrden;
            }
        }

        [HttpPost]
        public void Post([FromBody]Soda newSoda)
        {
            SDatabase.AddNewSoda(newSoda.Name, newSoda); //method insert
        }
        //------------------------------ END ENDPOINTS LAB 1 -------------------------------------------

        //------------------------------ ENDPOINTS LAB 3 and 4-----------------------------------------
        public Huffman hf = new Huffman();
        public LZW lzw = new LZW();
        private static readonly IFileComprassDataBase FDatabase = new FileCompressDataBase();

        // localhost:51626/weatherforecast/compress/Huffman/?Name=archivo
        [HttpPost("compress/Huffman", Name = "PostFile")]
        public async Task<string> Post(string Name, [FromForm]FileUploadAPI objFile)
        {
            try
            {
                if (objFile.files.Length > 0)
                {
                    if (!Directory.Exists(_environment.WebRootPath + "\\Upload\\"))
                    {
                        Directory.CreateDirectory(_environment.WebRootPath + "\\Upload\\");
                    }
                    using (FileStream fileStream = System.IO.File.Create(_environment.WebRootPath + "\\Upload\\" + objFile.files.FileName))
                    {
                        objFile.files.CopyTo(fileStream);
                        fileStream.Flush();
                        fileStream.Close();
                        string name = objFile.files.FileName.ToString();
                        string nameNew = GetNameNew(Name,".huff" );
                        string NewPath = _environment.WebRootPath + "\\Upload\\" + name;
                        //implement algorhitm
                        //completed stack
                        string metrics = "";
                        hf.Compress(@NewPath, @nameNew);
                        metrics = hf.GetFilesMetrics("file", @NewPath, @nameNew);
                        string[] metrics_total = metrics.Split('|', StringSplitOptions.RemoveEmptyEntries);
                        float rc = float.Parse(metrics_total[1]);
                        float fc = float.Parse(metrics_total[2]);
                        float rp = float.Parse(metrics_total[3]);
                        string algorithm = "Huffman";
                        FDatabase.AddNewFile(@NewPath, @nameNew, rc, fc, rp, algorithm);
                        return "\\Upload\\" + objFile.files.FileName;

                    }
                }
                else
                {
                    return "Failed";
                }     
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }

        // localhost:51626/weatherforecast/compress/LZW/?Name=archivo
        [HttpPost("compress/LZW", Name = "PostFile2")]
        public async Task<string> Post(string Name,string x, [FromForm]FileUploadAPI objFile)
        {
            try
            {
                if (objFile.files.Length > 0)
                {
                    if (!Directory.Exists(_environment.WebRootPath + "\\Upload\\"))
                    {
                        Directory.CreateDirectory(_environment.WebRootPath + "\\Upload\\");
                    }
                    using (FileStream fileStream = System.IO.File.Create(_environment.WebRootPath + "\\Upload\\" + objFile.files.FileName))
                    {
                        objFile.files.CopyTo(fileStream);
                        fileStream.Flush();
                        fileStream.Close();
                        string name = objFile.files.FileName.ToString();
                        string nameNew = GetNameNew(Name, ".lzw");
                        string NewPath = _environment.WebRootPath + "\\Upload\\" + name;
                        //implement algorhitm
                        //completed stack
                        string metrics = "";
                        lzw.Compress(@NewPath, @nameNew);
                        metrics = lzw.GetFilesMetrics("file", @NewPath, @nameNew);
                        string[] metrics_total = metrics.Split('|', StringSplitOptions.RemoveEmptyEntries);
                        float rc = float.Parse(metrics_total[1]);
                        float fc = float.Parse(metrics_total[2]);
                        float rp = float.Parse(metrics_total[3]);
                        string algorithm = "LZW";
                        FDatabase.AddNewFile(@NewPath, @nameNew, rc, fc, rp, algorithm);
                        return "\\Upload\\" + objFile.files.FileName;
                    }
                }
                else
                {
                    return "Failed";
                }
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }

        // localhost:51626/weatherforecast/decompress/?Algorithm=archivo
        [HttpPost("decompress", Name = "PostFile3")]
        public async Task<string> Post(string Algorithm, int x, [FromForm]FileUploadAPI objFile)
        {
            try
            {
                if (objFile.files.Length > 0)
                {
                    if (!Directory.Exists(_environment.WebRootPath + "\\Upload\\"))
                    {
                        Directory.CreateDirectory(_environment.WebRootPath + "\\Upload\\");
                    }
                    using (FileStream fileStream = System.IO.File.Create(_environment.WebRootPath + "\\Upload\\" + objFile.files.FileName))
                    {
                        objFile.files.CopyTo(fileStream);
                        fileStream.Flush();
                        fileStream.Close();
                        string name = objFile.files.FileName.ToString();
                        string NewPath = _environment.WebRootPath + "\\Upload\\" + name;
                        if (Algorithm ==  "LZW")
                        {
                            if (name.Contains(".lzw"))
                            {
                                string namefile = GetFileName(name);
                                string wPath = "decompress" + namefile;
                                lzw.Uncompress(@NewPath,@wPath);
                                return "Uncompress successful lzw";
                            }
                            return "You need file .lzw";
                        }
                        else if (Algorithm == "Huffman")
                        {
                            if (name.Contains(".huff"))
                            {
                                string decompressH = "";
                                decompressH = hf.uncompress(@NewPath);
                                return decompressH;
                            }
                            return "You need file .huff";
                        }
                        else
                        {
                            return "Algorithm incorrect";
                        }

                    }
                }
                else
                {
                    return "Failed";
                }
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }

        // localhost:51626/weatherforecast/compressions
        [HttpGet("compressions", Name = "GetFile")]
        /*[Route("weatherforecast/Sodas/")]*/
        public IEnumerable<FileCompress> Get()
        {
            return FDatabase.GetFiles();
        }

        //get new path for file compress
        private string GetNameNew(string name, string ext)
        {
            string newname = name + ext;
            return newname;
        }

        //get new path the decompress
        private string GetFileName(string name)
        {
            string res = "";
            for (int i = 0; i < name.Length; i++)
            {
                if (name[i] != '.')
                {
                    res = res + name[i];
                }
                else if (name[i] == '.')
                {
                    res = res + name[i] + "txt";
                    i = name.Length;
                }
            }
            return res;
        }

        //------------------------------ END ENDPOINTS LAB 3 and 4 ------------------------------------

        //------------------------------ ENDPOINTS LAB 5 -----------------------------------------------

        private static readonly IMethod methodData = new Method();

        // localhost:51626/weatherforecast/cipher/?Cifrado=caesar
        [HttpPost("cipher", Name = "PostCipher")]
        public async Task<string> Post(string Cifrado,[FromBody]MethodClass newMethod)
        {
            try
            {
                switch (Cifrado)
                {
                    case "zigzag":
                        methodData.CipherZigZag(newMethod.rPath, newMethod.wPath, newMethod.key);
                        break;
                    case "caesar":
                        methodData.CipherCesar(newMethod.rPath, newMethod.wPath, newMethod.word);
                        break;
                    case "ruta":
                        if (newMethod.TypeRuta == "vertical")
                        {
                            methodData.CipherRutaV(newMethod.rPath, newMethod.wPath, newMethod.key);
                        }
                        else if (newMethod.TypeRuta == "spiral")
                        {
                            methodData.CipherRutaS(newMethod.rPath, newMethod.wPath, newMethod.key);
                        }
                        break;
                    default:
                        break;
                }
                return "Cipher successful";
                       
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }

        // localhost:51626/weatherforecast/decipher/?Decifrado=caesar
        [HttpPost("decipher", Name = "PostDecipher")]
        public async Task<string> Post(string Decifrado, [FromBody]MethodClass newMethod,int x)
        {
            try
            {
                switch (Decifrado)
                {
                    case "zigzag":
                        methodData.DecipherZigZag(newMethod.rPath, newMethod.wPath, newMethod.key);
                        break;
                    case "caesar":
                        methodData.DecipherCesar(newMethod.rPath, newMethod.wPath, newMethod.word);
                        break;
                    case "ruta":
                        if (newMethod.TypeRuta == "vertical")
                        {
                            methodData.DecipherRutaV(newMethod.rPath, newMethod.wPath, newMethod.key);
                        }
                        else if (newMethod.TypeRuta == "spiral")
                        {
                            methodData.DecipherRutaS(newMethod.rPath, newMethod.wPath, newMethod.key);
                        }
                        break;
                    default:
                        break;
                }
                return "Decipher successful";

            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }

        //------------------------------ END ENDPOINTS LAB 5 -------------------------------------------

        //------------------------------ ENDPOINTS LAB 6 -----------------------------------------------
        //------------------------------ END ENDPOINTS LAB 6 -------------------------------------------



    }
}
