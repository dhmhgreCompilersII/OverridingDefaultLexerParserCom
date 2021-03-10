using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace OverridingDefaultLexerParserCom {
    class Program {
        static void Main(string[] args) {
            StreamReader r = new StreamReader(args[0]);
            AntlrInputStream ar = new AntlrInputStream(r);
            CSVLexer lexer = new CSVLexer(ar);
            MyCommonTokenStream tokens = new MyCommonTokenStream(lexer);
            CSVParser parser = new CSVParser(tokens);
            IParseTree tree = parser.compileUnit();
            Console.WriteLine(tree.ToStringTree());
        }
    }
}
