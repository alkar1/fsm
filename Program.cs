using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Linq.Expressions;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace fsm {
    class Program {

        static Dictionary<string, List<KeyValuePair<string, string>>> fsm =
            new Dictionary<string, List<KeyValuePair<string, string>>>();

        static void ReadFsmDef(string fName) {
            var file = File.ReadAllLines(fName);
            int i = 0;
            List<KeyValuePair<string, string>> li = null;
            while (i < file.Length) {
                var s = file[i].Trim();
                var state = s;
                li = new List<KeyValuePair<string, string>>();
                i++;
                do {
                    s = file[i].Trim();
                    if (s != "") {
                        var w = s.Split(new Char[] { ' ', '\t' });
                        li.Add(new KeyValuePair<string, string>(w[0], w[1]));
                    }
                    i++;
                } while (s != "" && i < file.Length);
                fsm.Add(state, li);
            }
        }

        static void FsmParse(string s) {
            string state = "start";
            List<KeyValuePair<string, string>> li = null;
            for (int i = 0; i < s.Length; i++) {
                if (! fsm.TryGetValue(state, out li)) {
                    Console.WriteLine("Bląd ! , brak zdefiniowanego stanu " + state);
                    return;
                }
                Console.WriteLine("wczytano > "+ s.Substring(i, 1));
                string newState = "";
                foreach (var trans in li) {
                    string rx = "["+trans.Key+"]";
                    Regex r = new Regex(rx, RegexOptions.IgnoreCase);
                    Match m = r.Match(s.Substring(i,1));
                    if (m.Success) {
                        newState = trans.Value;
                        continue;
                    }                    
                }
                if (newState == "") {
                    Console.WriteLine("Błąd ! Brak przejścia do następnego stanu");
                    return;
                }
                state = newState;
            }
            if (state.EndsWith("end")) {
                Console.WriteLine("OK, prawidłowo rozpoznano ciag znaków.");
            }
            else {
                Console.WriteLine("Błąd ! Zakonczono na stanie przejsciowym");
            }
        }

        static void Main(string[] args) {
            if (args.Length < 2) {
                Console.WriteLine(
                    "Wywowaj program z argumentami: nazwa_pliku_z_definicją_automatu ciąg_znaków_wejściowych");
                return;
            }
            ReadFsmDef(args[0]);
            FsmParse(args[1]);            
            Console.WriteLine("koniec");
        }
    }
}
