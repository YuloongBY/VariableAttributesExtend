/**
 * The MIT License (MIT)
 *
 * Copyright (c) 2022 YuloongBY - Github: github.com/YuloongBY
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of
 * this software and associated documentation files (the "Software"), to deal in
 * the Software without restriction, including without limitation the rights to
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
 * the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
 * FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
 * IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using UnityEngine;

#if UNITY_EDITOR
using System;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
#endif

/// <summary>
/// Category
/// </summary>
#if UNITY_EDITOR
[AttributeUsage( AttributeTargets.Field )]
#endif
public class CategoryAttribute : PropertyAttribute
{
	public string name_              = null;
    public bool   isFoldout_         = false;
    public bool   useHorizontalLine_ = false;
    public string titleName_         = null;

    /// <summary>
    /// Category 
    /// </summary>
    /// <param name="_name">Category name</param>
    /// <param name="_isFoldout">Whether the initial state is foldout or not</param>
    /// <param name="_useHorizontalLine">Add a split line above this category</param>   
    /// <param name="_titleName">Add a title above this category ，"null" means not to use the title</param>
    public CategoryAttribute( string _name , bool _isFoldout = false , bool _useHorizontalLine = false , string _titleName = null )
	{
		this.name_              = _name;
        this.isFoldout_         = _isFoldout;
        this.useHorizontalLine_ = _useHorizontalLine;
        this.titleName_         = _titleName;
    }
}

/// <summary>
/// When you Rename or Hide an enumeration item, 
/// you need to add attribute or [Rename] attribute above the enumeration variable
/// </summary>
#if UNITY_EDITOR
[AttributeUsage( AttributeTargets.Field )]
#endif
public class OnlyEditEnumAttribute : PropertyAttribute
{
    /// <summary>
    /// When you Rename or Hide an enumeration item, 
    /// you need to add attribute or [Rename] attribute above the enumeration variable
    /// </summary>
    public OnlyEditEnumAttribute(){}
}

/// <summary>
/// Hide this enumeration item
/// </summary>
#if UNITY_EDITOR
[AttributeUsage( AttributeTargets.Field )]
#endif
public class HideEnumAttribute : PropertyAttribute
{
    /// <summary>
    /// Hide this enumeration item
    /// </summary>
    public HideEnumAttribute(){}
}

/// <summary>
/// Rename
/// </summary>
#if UNITY_EDITOR
[AttributeUsage( AttributeTargets.Field )]
#endif
public class RenameAttribute : PropertyAttribute
{
    public string name_      = "";
    public string htmlColor_ = "#000000";

    /// <summary> Rename </summary>
    /// <param name="_name">New name</param>
    public RenameAttribute( string _name )
    {
        this.name_ = _name;
#if UNITY_EDITOR
        this.htmlColor_ = EditorGUIUtility.isProSkin ? "#D0D0D0" : "#000000";
#else
        this.htmlColor_ = "#000000";
#endif
    }

    /// <summary> Rename </summary>
    /// <param name="_name">New name</param>
    /// <param name="_htmlColor">Text color (For example："#FFFFFF" or "black")</param>
    public RenameAttribute( string _name , string _htmlColor )
    {
        this.name_      = _name;
        this.htmlColor_ = _htmlColor;
    }
}

/// <summary>
/// Add a title above this variable
/// </summary>
#if UNITY_EDITOR
[AttributeUsage( AttributeTargets.Field )]
#endif
public class TitleAttribute : PropertyAttribute
{
    public string title_             = "";
    public string htmlColor_         = "#606060";
    public bool   useHorizontalLine_ = false;

    /// <summary>
    /// Add a title above this variable
    /// </summary>
    /// <param name="_title">Title name</param>
    /// <param name="_useHorizontalLine">Add a split line above this title</param>
    public TitleAttribute( string _title , bool _useHorizontalLine = false )
    {
        this.title_ = _title;
        this.useHorizontalLine_ = _useHorizontalLine;

#if UNITY_EDITOR
        this.htmlColor_ = EditorGUIUtility.isProSkin ? "#999999" : "#606060";
#else
        this.htmlColor_ = "#606060";
#endif
    }

    /// <summary>
    /// Add a title above this variable
    /// </summary>
    /// <param name="_title">Title name</param>
    /// <param name="_htmlColor">Text color (For example："#FFFFFF" or "black")</param>
    /// <param name="_useHorizontalLine">Add a split line above this title</param>
    public TitleAttribute( string _title , string _htmlColor , bool _useHorizontalLine = false )
    {
        this.title_ = _title;
        this.htmlColor_ = _htmlColor;
        this.useHorizontalLine_ = _useHorizontalLine;
    }
}

/// <summary>
/// Add a split line above this variable
/// </summary>
#if UNITY_EDITOR
[AttributeUsage( AttributeTargets.Field )]
#endif
public class HorizontalLineAttribute: PropertyAttribute
{
    /// <summary>
    /// Add a split line above this variable
    /// </summary>
    public HorizontalLineAttribute(){}
}

#if UNITY_EDITOR
[CustomPropertyDrawer( typeof(RenameAttribute))]
[CustomPropertyDrawer( typeof(OnlyEditEnumAttribute))]
public class RenameDrawer : PropertyDrawer
{
    public override void OnGUI( Rect _position , SerializedProperty _property , GUIContent _label )
    {
        //保存当前颜色
        Color defaultColor = EditorStyles.label.normal.textColor;

        // 替换属性名称        
        if( attribute is RenameAttribute )
        {
            //重命名
            RenameAttribute rename = (RenameAttribute)attribute;
            _label.text = rename.name_;
            
            //重绘GUI
            EditorStyles.label.normal.textColor = HtmlToColor.Change( rename.htmlColor_ );
            bool isElement = Regex.IsMatch( _property.displayName , "Element \\d+" );
            if( isElement ) _label.text = _property.displayName;
        }

        //枚举
        if( _property.propertyType == SerializedPropertyType.Enum )
        {
            //枚举内的重命名和隐藏是否生效
            bool isEnumEdit = attribute is OnlyEditEnumAttribute || attribute is RenameAttribute;
            DrawEnum( _position , _property , _label , isEnumEdit );
        }
        //一般
        else
        { 
            EditorGUI.PropertyField( _position , _property , _label , true );                            
        }

        //颜色重置
        EditorStyles.label.normal.textColor = defaultColor;
    }

    /// <summary>
    /// 绘制枚举类型
    /// </summary>
    private void DrawEnum( Rect _position , SerializedProperty _property , GUIContent _label , bool _isEditEnum )
    {
        EditorGUI.BeginChangeCheck();

        // 获取枚举相关属性
        Type type = fieldInfo.FieldType;
        string[] names = _property.enumNames;
        string[] values = new string[ names.Length ];
        Array.Copy( names , values , names.Length );
        while( type.IsArray ) type = type.GetElementType();

        //枚举可编辑
        if( _isEditEnum )
        {
            for( int i = 0 ; i < names.Length ; i++ )
            {
                FieldInfo info = type.GetField( names[ i ]);

                //枚举内容是否被隐藏
                HideEnumAttribute hideAtr = info.GetCustomAttribute( typeof( HideEnumAttribute ) , true ) as HideEnumAttribute;
                if( hideAtr != null )
                {
                    //被隐藏则将其设置为空
                    values[ i ] = null;
                }
                else
                {
                    //没有被隐藏则重命名
                    RenameAttribute[] renameAtts = (RenameAttribute[])info.GetCustomAttributes( typeof(RenameAttribute) , true );
                    if( renameAtts.Length != 0 )values[ i ] = renameAtts[ 0 ].name_;
                }
            }
        }

        //创造显示枚举列表
        List<string> drawEnum = new List<string>( values.Length );
        List<GUIContent> guiContentArray = new List<GUIContent>( values.Length );
        for( int nCnt = 0 ; nCnt < values.Length ; nCnt ++ )
        {
            if( values[ nCnt ]!= null )
            { 
                drawEnum.Add( values[ nCnt ]);
                guiContentArray.Add( new GUIContent( values[ nCnt ]));
            }            
        }
        
        //显示枚举索引初期设置
        if( drawEnumIdx < 0 )
        {
            //如果该索引的枚举可以被显示则带入到显示枚举索引，否则为0
            string value = values[ _property.enumValueIndex ];
            drawEnumIdx = value == null ? 0 : drawEnum.IndexOf( value );
        }

        // 重绘GUI
        //drawEnumIdx = EditorGUI.Popup(position, label.text, drawEnumIdx, drawEnum.ToArray());
        drawEnumIdx = EditorGUI.Popup(_position, _label, drawEnumIdx, guiContentArray.ToArray());

        //获取选中枚举内容
        string selectedEnum = drawEnum[ drawEnumIdx ];
        
        //获取该枚举内容在实际枚举中的索引
        int index = Array.IndexOf( values , selectedEnum );
        //设置枚举
        if( EditorGUI.EndChangeCheck() && index != -1 ){ _property.enumValueIndex = index;}
    }
    //显示用枚举的索引
    private int drawEnumIdx = -1;

    public override float GetPropertyHeight( SerializedProperty _property , GUIContent _label )
    {
        return EditorGUI.GetPropertyHeight( _property , _label , true );
    }
}
#endif

#if UNITY_EDITOR
[CustomPropertyDrawer( typeof(TitleAttribute))]
public class TitleAttributeDrawer : DecoratorDrawer
{
    // 文本样式
    private GUIStyle style = new GUIStyle( EditorStyles.label );

    public override void OnGUI( Rect _position )
    {
        // 获取Attribute
        TitleAttribute title = ( TitleAttribute )attribute;

        if( title.useHorizontalLine_ )
        {
            HorizontalLine.Create( -45 );
        }

        style.fixedHeight = title.useHorizontalLine_ ? 50 : 25;
        style.normal.textColor = HtmlToColor.Change( title.htmlColor_ );
        style.fontSize = 13;
        // 重绘GUI
        _position = EditorGUI.IndentedRect( _position );
        GUI.Label( _position , title.title_ , style );
    }

    public override float GetHeight()
    {
        TitleAttribute title = ( TitleAttribute )attribute;        
        return base.GetHeight() + ( title.useHorizontalLine_ ? 15 : 5 );
    }
}
#endif

#if UNITY_EDITOR
[CustomPropertyDrawer( typeof(HorizontalLineAttribute))]
public class HorizontalLineDrawer : DecoratorDrawer
{
    public override void OnGUI( Rect _position )
    {
        HorizontalLine.Create( -30 );
    }

    public override float GetHeight()
    {
        return base.GetHeight();
    }
}
#endif

#if UNITY_EDITOR
public static class HorizontalLine
{
    public static void Create( float _yOffset )
    {
        var rect = EditorGUILayout.BeginVertical();
        rect.height = 1;
        rect.y += _yOffset;
        Color color = EditorGUIUtility.isProSkin ? new Color( 0.1f , 0.1f , 0.1f , 1.0f ) : new Color( 0.5f , 0.5f , 0.5f , 1.0f );
        EditorGUI.DrawRect( rect , color );
        EditorGUILayout.EndVertical();
    }
}
#endif

#if UNITY_EDITOR
public static class HtmlToColor
{
    /// <summary>
    /// Html颜色转换为Color
    /// </summary>
    public static Color Change( string _hex )
    {
        // 编辑器默认颜色
        if( string.IsNullOrEmpty( _hex )) return new Color( 0.705f , 0.705f , 0.705f );

        // 转换颜色
        _hex = _hex.ToLower();
        if( _hex.IndexOf( "#" ) == 0 && _hex.Length == 7 )
        {
            int r = Convert.ToInt32( _hex.Substring( 1 , 2 ) , 16 );
            int g = Convert.ToInt32( _hex.Substring( 3 , 2 ) , 16 );
            int b = Convert.ToInt32( _hex.Substring( 5 , 2 ) , 16 );
            return new Color( r / 255.0f , g / 255.0f , b / 255.0f );
        }
        else if( _hex == "red" )
        {
            return Color.red;
        }
        else if( _hex == "green" )
        {
            return Color.green;
        }
        else if( _hex == "blue" )
        {
            return Color.blue;
        }
        else if( _hex == "yellow" )
        {
            return Color.yellow;
        }
        else if( _hex == "black" )
        {
            return Color.black;
        }
        else if( _hex == "white" )
        {
            return Color.white;
        }
        else if( _hex == "cyan" )
        {
            return Color.cyan;
        }
        else if( _hex == "gray" )
        {
            return Color.gray;
        }
        else if( _hex == "grey" )
        {
            return Color.grey;
        }
        else if( _hex == "magenta" )
        {
            return Color.magenta;
        }
        else if( _hex == "orange" )
        {
            return new Color( 1.0f, 165.0f / 255.0f , 0.0f );
        }
        return new Color( 0.705f , 0.705f , 0.705f );
    }
}
#endif

