# 一个基于XML词法分析实现的微小的解释器, 仅 233 行代码, 
## 实现了:
### 变量定义(var),
### 赋值(set),
### 条件判断(if),
### 循环(for,while),
### 算术运算(+,-,*,/,%),
### 函数(Func,RETURN,CALL),
### 输入(READLINE),
### 输出(PRINT,PRINTLN),

# An tiny interpreter,based on XML analysis, only 233 lines of code,
## Implement features:
### Variable definition (var);
### Assignment (set);
### Conditional judgment (if);
### Loop (for, while);
### Arithmetic operations (+, -, *, /,%);
### Functions (Func, RETURN, CALL);
### Input (READLINE);
### Output (PRINT, PRINTLN);

# 示例/Example
```csharp
var ticks = Environment.TickCount;
new Interpreter().Eval(@"
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
");
ticks = Environment.TickCount - ticks;
Console.WriteLine("done. elapse:{0} ms", ticks);
Console.ReadLine();
```
