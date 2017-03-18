using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using System.Xml; 
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;


namespace ConsoleApplication1
{
    class Program
    {
        public static double GetTemp(string Name, Dictionary<string, int> dict)
        {
            XPathDocument xp = new XPathDocument(String.Format("http://informer.gismeteo.by/rss/" + dict[Name] + ".xml"));
            XmlDocument doc = new XmlDocument();
            doc.Load(String.Format("http://informer.gismeteo.by/rss/" + dict[Name] + ".xml"));
            string pathToSave = "E:/Programms/step/wheather/";
            doc.Save(String.Format(pathToSave + Name + "_" + DateTime.Today.Day + "_" + DateTime.Today.Month + "_" + DateTime.Today.Year + ".xml"));
            XPathNavigator xn = xp.CreateNavigator();
            string temprt = " ";
            XPathNodeIterator iter1 = xn.Select("//item/description");
            while (iter1.MoveNext())
            {


                temprt += iter1.Current.Value;
            }
            Regex reg = new Regex(@"[\s-]\d[\.\d]\.");
            MatchCollection mc = reg.Matches(temprt);
            double avg=0;
            int tmp=0;
            int count = 0;
            foreach (Match m in mc)
            {
                string digit=null;
                char [] p = m.ToString().ToCharArray();
                for (int i = 0; i < p.Length; i++)
                {
                    if (Char.IsDigit(p[i]))
                    {
                        if (digit != null)
                        {
                            digit += p[i].ToString();
                        }
                        else digit = p[i].ToString();
                    }
                }
                tmp = Convert.ToInt32(digit);
                if (p[0] == '-')
                {
                    tmp = tmp * -1;
                }
                avg += tmp;
                count++;
            }
            avg = avg / count;
            Console.WriteLine("temp in " + Name + " = " + avg);
            
            return avg;
        }
        static void Main(string[] args)
        {
            int choice = 1;
            string path = "E:/Programms/step/c3/Коды городов.txt";
            StreamReader sr = new StreamReader(path, Encoding.Default);
            Regex FindCodes = new Regex(@"\b\S\S*");
            string text = sr.ReadToEnd();
            sr.Close();
            MatchCollection CollectionsOfCities = FindCodes.Matches(text);
            Dictionary<string, int> dict = new Dictionary<string, int>();
            List<string> li = new System.Collections.Generic.List<string>();
            foreach (Match m in CollectionsOfCities)
            {
                li.Add(m.ToString());
            }
            int j = 0;
            for (int i = 0; i < CollectionsOfCities.Count; i = i + 2)
            {
                dict.Add(li[i + 1], Int32.Parse(li[i]));
            }
            Directory.CreateDirectory("E:/Programms/step/wheather");
            string pathToSave="E:/Programms/step/wheather/";

            do
            {

                try
                {
                    Console.Clear();
                    Console.WriteLine("Welcome to the Wheather!");
                    Console.WriteLine("**************************");
                    Console.WriteLine("1.-Pogoda v gorode");
                    Console.WriteLine("2.-Spisok dostupnih gorodov");
                    Console.WriteLine("3.-Dobavit' gorod");
                    Console.WriteLine("4.-Statistika v gorode za period");
                    Console.WriteLine("5.-Srednyaya temperatura v gorodah v spiske");
                    Console.WriteLine("0.-Vihod");
                    choice = Convert.ToInt32(Console.ReadLine());
                    switch (choice)
                    {


                        case 1:
                            Console.Clear();
                            Console.WriteLine("Введите название города ");
                            string CityName = Console.ReadLine();
                            Dictionary<string, int>.KeyCollection Cities = dict.Keys;
                            if (Cities.Contains(CityName))
                            {
                                XPathDocument xp = new XPathDocument(String.Format("http://informer.gismeteo.by/rss/" + dict[CityName] + ".xml"));
                                XmlDocument doc = new XmlDocument();
                                doc.Load(String.Format("http://informer.gismeteo.by/rss/" + dict[CityName] + ".xml"));
                                doc.Save(String.Format(pathToSave + CityName + "_" + DateTime.Today.Day+"_"+DateTime.Today.Month+"_"+DateTime.Today.Year + ".xml"));

                                XPathNavigator xn = xp.CreateNavigator();
                                XPathNodeIterator iterator = xn.Select("//item/title");
                                XPathNodeIterator iter1 = xn.Select("//item/description");
                                while (iterator.MoveNext() && iter1.MoveNext())
                                {


                                    Console.WriteLine(iterator.Current.Value);
                                    Console.WriteLine(iter1.Current.Value);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Net takogo goroda v spiske");
                            }
                            break;


                        case 2:
                            Console.Clear();
                            foreach (KeyValuePair<string, int> pair in dict)
                            {
                                Console.WriteLine(pair.Key);
                            }

                            break;
                        case 3:
                            Console.Clear();
                            Console.WriteLine("Vvedite imya goroda");
                            string newCityName = Console.ReadLine();
                            Dictionary<string, int>.KeyCollection temp = dict.Keys;
                            if (!temp.Contains(newCityName))
                            {
                                Console.WriteLine("Vvedite kod goroda");
                                int NewCode = Convert.ToInt32(Console.ReadLine());
                                XmlDocument tmpdoc = new XmlDocument();
                                tmpdoc.Load(String.Format("http://informer.gismeteo.by/rss/" + NewCode + ".xml"));
                                dict.Add(newCityName, NewCode);
                                StreamWriter tempsw = new StreamWriter(path, true, Encoding.Default);
                                tempsw.WriteLine(String.Format(NewCode + "  " + newCityName));
                                tempsw.Close();
                                tempsw.Dispose();
                               
                            }
                            else
                            {
                                Console.WriteLine("gorod uzhe dobavlen!");
                            }
                            break;
                        case 5:
                            Console.Clear();
                            double average;
                            int count = 0;
                            double sum = 0;
                            foreach (KeyValuePair<string, int> para in dict)
                            {
                                sum += GetTemp(para.Key, dict);
                                count++;
                            }
                            average = sum / count;
                            Console.WriteLine(average);
                            break;
                        case 0: break;

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                    continue;
                }
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
               
            } while (choice != 0);
            
        }
    }
}
