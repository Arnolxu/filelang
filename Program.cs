using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

namespace FileLang // 0.3
{
    public class Program
    {
        public static void Main(string[] args)
        {
		    Program prg = new Program();
		    string path = "";
		    if(args.Length>=2)
		    {
		    	Console.WriteLine("Please don't enter more than 1 argument.");
		    	System.Environment.Exit(1);
		    }
		    if(args.Length!=1 && !File.Exists("main.fl"))
		    {
		    	Console.WriteLine("Please enter a path.");
		    	System.Environment.Exit(0);
		    }
		    else
		    {
		        if(args.Length==1){
                	if(File.Exists(args[0]))
		    	    	path = args[0];
		    	    else
		    	    	path = args[0] + "/main.fl";
                } else {
                    path = "main.fl";
                }
		    }
		    if(!File.Exists(@path)){
		    	Console.WriteLine("Invalid path: " + path + ".\nNote: The path should be a directory containing a file named main.fl, not a .fl file.");
		    	System.Environment.Exit(0);
		    }
		    Dictionary<string, string> vars = new Dictionary<string, string>();
		    FileLang(File.ReadAllLines(@path), Path.GetDirectoryName(Path.GetFullPath(@path)), vars);
        }
        public static void FileLang(string[] file, string path, Dictionary<string, string> vars)
        {
        	string[] cmp = new string[] {"", ""};
        	bool cmping = false;
		    int nline = 1;
            string sfile = Path.GetFileName(path);
            foreach(string lin in file){
			    MatchEvaluator evaluator = new MatchEvaluator(blank);
			    string line = Regex.Replace(lin, "#(?=([^\"]*\"[^\"]*\")*[^\"]*$).*", evaluator);
			    line = Regex.Replace(line, "^[ \t]+", evaluator);
			    string[] words = line.Split(' ');
			    if(line=="")
				    continue;
                if(words[0]=="use")
                    FileLang(File.ReadAllLines(path + "/" + @words[1] + ".fl"), path, vars);
                else if(words[0]=="set")
                    addOrUpdate(vars, words[1], string.Join(" ", words.Skip(2).ToArray()).Replace("\\n", "\n"));
                else if(words[0]=="out")
                    Console.Write(string.Join(" ", words.Skip(1).ToArray()).Replace("\\n", "\n"));
                else if(words[0]=="outv")
                    Console.Write(vars[words[1]]);
                else if(words[0]=="in")
                    addOrUpdate(vars, words[1], Console.ReadLine());
                else if(words[0]=="os"){
                    var process = new Process();
                    process.StartInfo.FileName = "sh";
                    process.StartInfo.Arguments = "-c \"" + String.Join(" ", words.Skip(2).ToArray()) + "\"";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.Start();
                    process.WaitForExit();
                }
                else if(words[0]=="osv"){
                    var process = new Process();
                    process.StartInfo.FileName = "sh";
                    process.StartInfo.Arguments = "-c \"" + vars[words[1]] + "\"";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.Start();
                    process.WaitForExit();
                }
                else if(words[0]=="os_out"){
                    var process = new Process();
                    process.StartInfo.FileName = "sh";
                    process.StartInfo.Arguments = "-c \"" + String.Join(" ", words.Skip(2).ToArray()) + "\"";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.Start();

                    StreamReader reader = process.StandardOutput;
                    string output = reader.ReadToEnd();

                    addOrUpdate(vars, words[1], output);
                }
                else if(words[0]=="os_outv"){
                    var process = new Process();
                    process.StartInfo.FileName = "sh";
                    process.StartInfo.Arguments = "-c \"" + vars[words[2]] + "\"";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.Start();

                    StreamReader reader = process.StandardOutput;
                    string output = reader.ReadToEnd();

                    addOrUpdate(vars, words[1], output);
                }
                else if(words[0]=="re"){
                    Regex rgx = new Regex(String.Join(" ", words.Skip(4).ToArray()));
                    addOrUpdate(vars, words[1], rgx.Matches(vars[words[2]])[Int32.Parse(words[3]) - 1].Groups[words[3]].Captures[0].Value);
                }
                else if(words[0]=="add"){
					int a;
					int b;
					Int32.TryParse(vars[words[1]], out a);
					if(!Int32.TryParse(words[2], out b))
						Int32.TryParse(vars[words[2]], out b);
					vars[words[1]] = (a + b).ToString();
				}
                else if(words[0]=="sub"){
					int a;
					int b;
					Int32.TryParse(vars[words[1]], out a);
					if(!Int32.TryParse(words[2], out b))
						Int32.TryParse(vars[words[2]], out b);
					vars[words[1]] = (a - b).ToString();
				}
                else if(words[0]=="div"){
					int a;
					int b;
					Int32.TryParse(vars[words[1]], out a);
					if(!Int32.TryParse(words[2], out b))
						Int32.TryParse(vars[words[2]], out b);
					vars[words[1]] = (a / b).ToString();
				}
                else if(words[0]=="mlt"){
					int a;
					int b;
					Int32.TryParse(vars[words[1]], out a);
					if(!Int32.TryParse(words[2], out b))
						Int32.TryParse(vars[words[2]], out b);
					vars[words[1]] = (a * b).ToString();
				}
                else if(words[0]=="round"){
					vars[words[1]] = Math.Round(Double.Parse(vars[words[1]])).ToString();
				}
                else if(words[0]=="mod"){
					int a;
					int b;
					Int32.TryParse(vars[words[1]], out a);
					if(!Int32.TryParse(words[2], out b))
						Int32.TryParse(vars[words[2]], out b);
					vars[words[1]] = (a % b).ToString();
				}
                else if(words[0]=="cmp"){
					cmp = new string[] {vars[words[1]], vars[words[2]]};
					cmping = true;
				}
                else if(cmping){
                	if(words[0]=="ue"){
						if(cmp[0] == cmp[1])
							FileLang(File.ReadAllLines(path + "/" + @words[1] + ".fl"), path, vars);

						cmping = false;
						cmp = new string[] {"", ""};
					}
                	else if(words[0]=="une"){
						if(cmp[0] != cmp[1])
							FileLang(File.ReadAllLines(path + "/" + @words[1] + ".fl"), path, vars);

						cmping = false;
						cmp = new string[] {"", ""};
					}
                	else if(words[0]=="ug"){
						if(Int32.Parse(cmp[0]) > Int32.Parse(cmp[1]))
							FileLang(File.ReadAllLines(path + "/" + @words[1] + ".fl"), path, vars);

						cmping = false;
						cmp = new string[] {"", ""};
					}
                	else if(words[0]=="uge"){
						if(Int32.Parse(cmp[0]) >= Int32.Parse(cmp[1]))
							FileLang(File.ReadAllLines(path + "/" + @words[1] + ".fl"), path, vars);

						cmping = false;
						cmp = new string[] {"", ""};
					}
                	else if(words[0]=="ul"){
						if(Int32.Parse(cmp[0]) < Int32.Parse(cmp[1]))
							FileLang(File.ReadAllLines(path + "/" + @words[1] + ".fl"), path, vars);

						cmping = false;
						cmp = new string[] {"", ""};
					}
                	else if(words[0]=="ule"){
						if(Int32.Parse(cmp[0]) <= Int32.Parse(cmp[1]))
							FileLang(File.ReadAllLines(path + "/" + @words[1] + ".fl"), path, vars);

						cmping = false;
						cmp = new string[] {"", ""};
					}
				}
                else if(words[0]=="env"){
					addOrUpdate(vars, words[1], Environment.GetEnvironmentVariable(words[2]));
				}
                else if(words[0]=="basename"){
					addOrUpdate(vars, words[1], Path.GetFileName(vars[words[1]]));
				}
                else if(words[0]=="exit"){
		    		System.Environment.Exit(0);
				}
                else if(words[0]=="substr"){
		    		addOrUpdate(vars, words[1], vars[words[1]].Substring(Int32.Parse(words[2]), Int32.Parse(words[3])));
				}
                else
                    Console.WriteLine("Error in file '" + sfile + "', on line " + nline.ToString() + ": Command " + words[0] + " not found.");
                nline++;
            }
        }
	    public static string blank(Match match){
	    	return "";
	    }
	    public static void addOrUpdate(Dictionary<string, string> dic, string key, string newValue)
	    {
    	    string val;
    	    if (dic.TryGetValue(key, out val))
    	    {
        	    // Exists
        	    dic[key] = newValue;
    	    }
    	    else
    	    {
        	    // Doesn't exist
        	    dic.Add(key, newValue);
    	    }
	    }
    }
}
