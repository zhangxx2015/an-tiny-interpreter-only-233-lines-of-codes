using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
namespace XML_based_interpreter {
    /// <summary>
    /// 一个基于XML词法分析实现的微小的解释器, 仅 233 行代码, 
    /// 实现了:
    ///     变量定义(var),
    ///     赋值(set),
    ///     条件判断(if),
    ///     循环(for,while),
    ///     算术运算(+,-,*,/,%),
    ///     函数(Func,RETURN,CALL),
    ///     输入(READLINE),
    ///     输出(PRINT,PRINTLN),
    /// 
    /// An tiny interpreter,based on XML analysis, only 233 lines of code,
    /// Implement features:
    ///        Variable definition (var);
    ///        Assignment (set);
    ///        Conditional judgment (if);
    ///        Loop (for, while);
    ///        Arithmetic operations (+, -, *, /,%);
    ///        Functions (Func, RETURN, CALL);
    ///        Input (READLINE);
    ///        Output (PRINT, PRINTLN);
    /// </summary>
    internal static class XmlExpend {
        internal static Dictionary<string, string> Attrs(this XmlNode node) {
            if (null == node.Attributes) throw new Exception("Undefined var name");
            return node.Attributes.Cast<XmlAttribute>().ToDictionary(item => item.Name, item => item.Value);
        }
        internal static XmlAttribute Attr(this XmlNode node, string name) {
            if (null == node.Attributes) throw new Exception("Undefined var name");
            var attr = node.Attributes.Cast<XmlAttribute>().FirstOrDefault(item => item.Name == name);
            if (null == attr) throw new Exception("Undefined var name");
            return attr;
        }
    }
    internal static class VarsExtent {
        private static dynamic ParseValue(string value) {
            bool b;
            if (bool.TryParse(value, out b)) return b;
            int i;
            if (int.TryParse(value, out i)) return i;
            float f;
            if (float.TryParse(value, out f)) return f;
            return value;
        }
        internal static Dictionary<string, object> New(this Dictionary<string, object> vars, string name, string value) {
            vars.Add(name, ParseValue(value));
            return vars;
        }
        internal static Dictionary<string, object> Set(this Dictionary<string, object> vars, string name, string value) {
            if (!vars.ContainsKey(name)) throw new Exception("Var is not exist");
            vars[name] = vars.Exist(value) ? vars.Get(value) : ParseValue(value);
            return vars;
        }
        internal static dynamic Get(this Dictionary<string, object> vars, string name) {
            if (!vars.ContainsKey(name)) throw new Exception("Var is not exist");
            return vars[name];
        }
        internal static bool Exist(this Dictionary<string, object> vars, string name) {
            return vars.ContainsKey(name);
        }
        internal static bool Compare(this Dictionary<string, object> vars, string v1, string compareTypes, string v2) {
            return new Dictionary<string, Func<float, float, bool>> {
                {"LT", (p1, p2) => p1 < p2},
                {"LE", (p1, p2) => p1 <= p2},
                {"EQ", (p1, p2) => Equals(p1, p2)},
                {"GT", (p1, p2) => p1 > p2},
                {"GE", (p1, p2) => p1 >= p2},
                {"NE", (p1, p2) => !Equals(p1, p2)},
            }[compareTypes](vars.ContainsKey(v1) ? vars.Get(v1) : ParseValue(v1), vars.ContainsKey(v2) ? vars.Get(v2) : ParseValue(v2));
        }
        internal static dynamic Arithmetic(this Dictionary<string, object> vars, string v1, string arithmeticTypes, string v2) {
            return new Dictionary<string, Func<float, float, float>> {
                {"ADD", (p1, p2) => p1 + p2},
                {"SUB", (p1, p2) => p1 - p2},
                {"MUL", (p1, p2) => p1 * p2},
                {"DIV", (p1, p2) => p1 / p2},
                {"MOD", (p1, p2) => p1 % p2},
            }[arithmeticTypes](vars.ContainsKey(v1) ? vars.Get(v1) : ParseValue(v1), vars.ContainsKey(v2) ? vars.Get(v2) : ParseValue(v2));
        }
    }
    /// <summary>
    /// XML based program interpreter by QQ:20437023, MIT License Copyright (c) 2017 zhangxx2015
    /// </summary>
    public class Interpreter {
        public readonly Dictionary<string, object> Vars = new Dictionary<string, object>();
        public readonly Dictionary<string, string> Func = new Dictionary<string, string>();
        public void Reset() {
            Vars.Clear();
            Func.Clear();
        }
        private bool _terminate;
        public void Eval(string codes) {
            var xdoc = new XmlDocument();
            xdoc.LoadXml(codes);
            var root = xdoc.DocumentElement;
            if (null == root) throw new Exception("Failed to read");
            foreach (XmlNode node in root.ChildNodes) {
                if (_terminate) break;
                var inc = node.Name;
                var attrs = node.Attrs();
                var def = attrs.FirstOrDefault();
                switch (inc) {
                    case "VAR":
                        Vars.New(def.Key, def.Value);
                        break;
                    case "IF_LT":
                    case "IF_LE":
                    case "IF_EQ":
                    case "IF_GT":
                    case "IF_GE":
                    case "IF_NE":
                        if (Vars.Compare(def.Key, inc.Replace("IF_", string.Empty), def.Value))
                            Eval(string.Format("<SEGMENT>{0}</SEGMENT>", node.InnerXml));
                        break;
                    case "FOR_LT":
                    case "FOR_LE":
                    case "FOR_EQ":
                    case "FOR_GT":
                    case "FOR_GE":
                    case "FOR_NE":
                        Vars.New("TO", attrs["TO"]).New(def.Key, def.Value);
                        while (Vars.Compare(def.Key, inc.Replace("FOR_", string.Empty), "TO")) {
                            Eval(string.Format("<SEGMENT>{0}</SEGMENT>", node.InnerXml));
                            float index = Vars.Get(def.Key);
                            index++;
                            Vars.Set(def.Key, string.Format("{0}", index));
                        }
                        break;
                    case "SET":
                        Vars.Set(def.Key, def.Value);
                        break;
                    case "ADD":
                    case "SUB":
                    case "MUL":
                    case "DIV":
                    case "MOD":
                        Vars.Set(def.Key, (string)Vars.Arithmetic(def.Key, inc, def.Value).ToString());
                        break;
                    case "WHILE_LT":
                    case "WHILE_LE":
                    case "WHILE_EQ":
                    case "WHILE_GT":
                    case "WHILE_GE":
                    case "WHILE_NE":
                        while (Vars.Compare(def.Key, inc.Replace("WHILE_", string.Empty), def.Value))
                            Eval(string.Format("<SEGMENT>{0}</SEGMENT>", node.InnerXml));
                        break;
                    case "READLINE":
                        if (!Vars.Exist(def.Key)) throw new Exception("Var is not exist");
                        Vars.Set(def.Key, Console.ReadLine());
                        break;
                    case "PRINT":
                    case "PRINTLN":
                        var msg = def.Value;
                        if (Vars.Exist(def.Value)) msg = Vars.Get(def.Value).ToString();
                        if (inc == "PRINTLN") {
                            Console.WriteLine(msg);
                            break;
                        }
                        Console.Write(msg);
                        break;
                    case "RETURN":
                        _terminate = true;
                        break;
                    case "FUNC":
                        foreach (var arg in attrs)
                            Vars.New(arg.Key, string.Empty);
                        Func.Add(def.Key, string.Format("<SEGMENT>{0}</SEGMENT>", node.InnerXml));
                        break;
                    case "CALL":
                        if (attrs.Count < 1) throw new Exception("Params is not enough");
                        var funName = def.Value;
                        var outVar = def.Key;
                        if (!Func.ContainsKey(funName)) throw new Exception("Function is not exist");
                        attrs.Remove(outVar);
                        foreach (var attr in attrs)
                            Vars.Set(attr.Key, attr.Value);
                        Eval(Func[funName]);
                        Vars.Set(outVar, funName);
                        break;
                }
            }
        }
    }
    class Program {
        static void Main() {
            const string examples = @"
<PROGRAM>
    <PRINT V='please input your name:' />
    <VAR str='guest' />
    <READLINE str='' />
    <PRINT V='hello!' />
    <PRINTLN V='str' />

	<FOR_LT i='0' TO='3'>
        <IF_GE i='2'>
            <PRINTLN V='have a good day!' />
        </IF_GE>
        <PRINT V='welcome!' />
		<PRINTLN V='str' />
	</FOR_LT>
    <PRINT V='i=' />
    <PRINTLN V='i' />
    <WHILE_GT i='0'>
        <SUB i='1' />
        <PRINT V='i--=' />
        <PRINTLN V='i' />
    </WHILE_GT>
    <FUNC add='FLOAT' x='FLOAT' y='FLOAT'>
        <SET add='x' />
        <ADD add='y' />
    </FUNC>
    <VAR f='0' />
    <CALL f='add' x='123' y='456' />
    <PRINT V='call func add(123,456)=' />
    <PRINT V='123+456=' />
    <PRINTLN V='f' />
</PROGRAM>
";
            var ticks = Environment.TickCount;
            new Interpreter().Eval(examples);
            ticks = Environment.TickCount - ticks;
            Console.WriteLine("done. elapse:{0} ms", ticks);
            Console.ReadLine();
        }
    }
}
