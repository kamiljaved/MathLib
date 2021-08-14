
<h1 align="center">
  <br>
  MathLib
  <br>
</h1>

<h4 align="center">Symbolic Math App in C# (Windows Forms GUI 
App). Supports storing, evaluating, and taking derivatives of Symbolic Math Expressions.</h4>

<p></p>

<p align="center">
  <a href="https://easybase.io">
    <img src="https://raw.githubusercontent.com/kamiljaved98/MathLib/master/mlgui_ico.ico" alt="easybase logo black" width="80" height="80">
  </a>
</p>

<p align="center">
	<a href="https://docs.microsoft.com/en-us/dotnet/csharp/">
		<img src="https://forthebadge.com/images/badges/made-with-c-sharp.svg" alt="Uses C#." title="Uses C#."/>
	</a>
  <a>
		<img src="https://forthebadge.com/images/badges/60-percent-of-the-time-works-every-time.svg" alt=" Made with Python.">
  </a>
  <a>
		<img src="https://forthebadge.com/images/badges/built-with-love.svg" height="30px" alt="Uses HTML.">
  </a>

</p>

<p align="center">
  <a href="#disclaimer">Disclaimer</a> •
  <a href="#about">About</a> •
  <a href="#key-features">Key Features</a> •
  <a href="#how-to-use">How To Use</a> •
  <a href="#notes">Notes</a>
</p>

<hr>

## Disclaimer

This app is by no means a complete product, but is only meant for demo purposes. It is prone to logical flaws in case of use outside its intended functionality described below.

## About

MathLib is a Windows Forms App that allows storing, evaluating, and taking derivatives of Symbolic Math Expressions.
<p>Implemented using Data Structures and OOP concepts. Uses Stacks to parse the expression into Postfix form, to generate a Function Tree (an Object). Each Function’s behaviour is 
described by properties and methods in its Class, while all common & abstract methods reside in a Parent ‘Function’ class.</p> 
Minor function simplification has been implemented but not enabled in this GUI version.

## Key Features

* Store mathematical expressions with a function name
* Manipulate the functions by substituting them into other expressions as well as within themselves
* Evaluate function values at desired variable values
* Take derivatives of functions (symbolically as well as at numeric points)
* View the generated function-tree
<p>It provides a number of math operators (such as + - * / ^), and some common math functions (such as sin, cos, ln, pow).</p>

## How To Use

You'll need [Visual Studio](https://www.python.org/) with C# development support to build this app. From your command line:

```bash
# Clone this repository (or download from github page)
$ git clone https://github.com/kamiljaved98/MathLib

# Go into the repository
$ cd MathLib
```
Then run <b>MathLib.csproj</b> project file with Visual Studio. A Release version is also present in <b>bin/Release</b> folder. 
Alternatively, <a href="https://drive.google.com/file/d/1A9Jr5JbvYjjOIRObRO-J7vkxZN8-4q7t/view?usp=sharing"> get the app here</a>.
<p>A short usage example is given below. </p>

```m
y = x + a + b
```

This input declares <b>y</b> as a new function over the variables <b>x</b>, <b>a</b>, and <b>b</b>. Its generated function-tree can be viewed by clicking on the <i>Function Tree</i> tab in the bottom dialog and then selecting <i>y</i> from the list of Functions on bottom right.

## Notes

<p>Last code update: <b>December 2018</b></p>
<p>Last README update: <b>August 2021</b></P>


---

> [kamiljaved.pythonanywhere.com](https://kamiljaved.pythonanywhere.com/) &nbsp;&middot;&nbsp;
> GitHub [@kamiljaved98](https://github.com/kamiljaved98)
