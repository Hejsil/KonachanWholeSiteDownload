using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Serializers;

namespace KonachanWholeSiteDownload
{
    class Program
    {
        static void Main(string[] args)
        {
            var url = @"http://konachan.net/";
            var client = new RestClient(url);

            var pageErrors = new ConcurrentBag<int>();
            var errorImages = new ConcurrentBag<Image>();
            Parallel.For(1, 8651, i =>
            {
                try
                {
                    var response = client.Get(new RestRequest($"post?page={i}"));
                    var res = Newtonsoft.Json.JsonConvert.DeserializeObject<Image[]>(response.Content);

                    foreach (var item in res)
                    {
                        try
                        {
                            var webClient = new WebClient();
                            var ext = item.file_url.Split('.').LastOrDefault();

                            if (ext == null)
                                ext = "png";

                            webClient.DownloadFile(item.file_url, $@"{args[0]}\{item.id}.{ext}");
                        }
                        catch (Exception)
                        {
                            errorImages.Add(item);
                        }
                    }
                }

                catch
                    (Exception)
                {
                    pageErrors.Add(i);
                }
            });

            File.WriteAllLines("ErrorPages.txt", pageErrors.Select(x => x.ToString()));
            File.WriteAllLines("ErrorImages.txt", errorImages.Select(x => x.file_url));
            Console.WriteLine("Done!");
            Console.ReadKey();
        }
    }

    public class Image
    {
        public int id { get; set; }
        public string tags { get; set; }
        //public int? created_at { get; set; }
        //public int? creator_id { get; set; }
        //public string author { get; set; }
        //public int change { get; set; }
        //public string source { get; set; }
        //public int score { get; set; }
        //public string md5 { get; set; }
        //public int file_size { get; set; }
        public string file_url { get; set; }
        //public bool is_shown_in_index { get; set; }
        //public string preview_url { get; set; }
        //public int preview_width { get; set; }
        //public int preview_height { get; set; }
        //public int actual_preview_width { get; set; }
        //public int actual_preview_height { get; set; }
        //public string sample_url { get; set; }
        //public int sample_width { get; set; }
        //public int sample_height { get; set; }
        //public int sample_file_size { get; set; }
        //public string jpeg_url { get; set; }
        //public int jpeg_width { get; set; }
        //public int jpeg_height { get; set; }
        //public int jpeg_file_size { get; set; }
        //public string rating { get; set; }
        //public bool has_children { get; set; }
        //public object parent_id { get; set; }
        //public string status { get; set; }
        //public int width { get; set; }
        //public int height { get; set; }
        //public bool is_held { get; set; }
        //public string frames_pending_string { get; set; }
        //public List<object> frames_pending { get; set; }
        //public string frames_string { get; set; }
        //public List<object> frames { get; set; }
        //public object flag_detail { get; set; }

        public override int GetHashCode() => id;

        public override bool Equals(object obj)
        {
            var other = obj as Image;
            if (other != null)
                return id == other.id;

            return base.Equals(obj);
        }
    }
}
