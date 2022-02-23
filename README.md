[![license](https://img.shields.io/badge/license-MIT-brightgreen.svg?style=flat-square)](https://github.com/YuloongBY/VariableAttributesExpand/blob/main/LICENSE)

# VariableAttributesExpand

add properties Variables can be collapsed, renamed, added with split lines and titles , Enumeration items can be individually hidden

添加属性，可对变量进行折叠，重命名，添加分割线，添加标题操作，可对枚举项进行单独隐藏

![Image](https://github.com/YuloongBY/BYImage/blob/main/VariableAttributesExtend/StyleDark.png)
![Image](https://github.com/YuloongBY/BYImage/blob/main/VariableAttributesExtend/StyleLight.png)

## Default
Displays the class in which the variable is declared

![Image](https://github.com/YuloongBY/BYImage/blob/main/VariableAttributesExtend/DrawClass.png)
```csharp
public class SampleBase : MonoBehaviour
{
    public int parameter0 = 0;
    public int parameter1 = 0;
    public int parameter2 = 0;
}
```
You can view the original variable name where the mouse hovers

![Image](https://github.com/YuloongBY/BYImage/blob/main/VariableAttributesExtend/Mouse.gif)

## How to use
Add corresponding attributes to the displayable variables

### Rename
Rename the displayable variables

![Image](https://github.com/YuloongBY/BYImage/blob/main/VariableAttributesExtend/Rename.png)
```csharp
public class SampleBase : MonoBehaviour
{
    //Rename
    [Rename("#Param0")]
    public int parameter0 = 0;

    //Rename + Color
    [Rename("#Param1", "red")]
    public int parameter1 = 1;
}
```
### HorizontalLine
Add a split line above the displayable variables

![Image](https://github.com/YuloongBY/BYImage/blob/main/VariableAttributesExtend/Splitline.png)
```csharp
public class SampleBase : MonoBehaviour
{
    //HorizontalLine
    [HorizontalLine]
    public int parameter0 = 0;
}
```
### Title
Add a title above the displayable variables

![Image](https://github.com/YuloongBY/BYImage/blob/main/VariableAttributesExtend/Title.png)
```csharp
public class SampleBase : MonoBehaviour
{
    //Title
    [Title("[Title0]")]
    public int parameter0 = 0;

    //Title + SplitLine
    [Title("[Title1]" , true )]
    public int parameter1 = 1;

    //Title + Color + SplitLine
    [Title("[Title2]", "red" , true)]
    public int parameter2 = 2;
}
```
### HideEnum
Hide the enumeration item

![Image](https://github.com/YuloongBY/BYImage/blob/main/VariableAttributesExtend/Enum.png)
```csharp
public class SampleBase : MonoBehaviour
{
    public enum SAMPLE_ENUM
    {
        ENUM_0 = 0 ,
        //HideEnum
        [HideEnum]
        ENUM_1 ,
        //Rename
        [Rename("#Enum2")]
        ENUM_2 
    }

    //When you Rename or Hide an enumeration item,
    //you need to add [OnlyEditEnum] or [Rename] attribute above the enumeration variable
    [OnlyEditEnum]
    public SAMPLE_ENUM parameter0 = 0;
}
```
### Category
Add categories to variable

![Image](https://github.com/YuloongBY/BYImage/blob/main/VariableAttributesExtend/Category.gif)
```csharp
public class SampleBase : MonoBehaviour
{
    //Category
    [Category("Cat1")]
    public int parameter0 = 0;
    [Category("Cat1")]
    public int parameter1 = 1;

    //Category + Foldout
    [Category("Cat1|Child1" , true )]
    public int parameter2 = 2;
    [Category("Cat1|Child2")]
    public int parameter3 = 3;

    //Category + HorizontalLine + Title
    [Category("Cat2" , true , true , "[Title]")]
    public int parameter4 = 4;
}
```
