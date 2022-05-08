using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor( typeof( UnityEngine.Object ) , true , isFallback = true )]
[CanEditMultipleObjects]
public class VariableAttributesExtendEditor : Editor
{
	private Dictionary<Type, Attributes> attributesArray_ = new Dictionary<Type, Attributes>();
	private List<FieldInfo> objectFields_;
	private bool initialized_;
	private SerializedProperty mSprictProp_;

	private GUIStyle scriptTagStyle_;
	private GUIStyle categorryTitleStyle_;

	private void Awake()
	{
        {
			scriptTagStyle_ = new GUIStyle( EditorStyles.label );
			scriptTagStyle_.fontSize = 9;
			scriptTagStyle_.normal.textColor = EditorGUIUtility.isProSkin ? new Color( 0.4f , 0.4f , 0.4f ) : new Color( 0.2f , 0.2f , 0.2f );
		}
		{
			categorryTitleStyle_ = new GUIStyle( EditorStyles.label );
			categorryTitleStyle_.fontSize = 13;
			categorryTitleStyle_.normal.textColor = EditorGUIUtility.isProSkin ? new Color( 0.6f , 0.6f , 0.6f ) : new Color( 0.38f , 0.38f , 0.38f );
		}
	}

	void OnEnable()
	{
		// 获取类型和可序列化属性
		Type type = target.GetType();
		List<FieldInfo> fields = new List<FieldInfo>();
		FieldInfo[] array = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
		fields.AddRange(array);

		var types = new List<Type>();
		// 获取父类的可序列化属性
		while( IsTypeCompatible( type.BaseType ) && type != type.BaseType )
		{
			types.Add( type );
			type = type.BaseType;
			array = type.GetFields( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly );
			fields.InsertRange( 0 , array );
		}

		if( fields.Count > 0 )
        {
			//删除非序列化内容
			for( int nCnt = fields.Count - 1 ; nCnt >= 0 ; nCnt-- )
			{
				// 非公有并且没有添加[SerializeField]特性的属性
				if( !fields[ nCnt ].IsPublic )
				{
					object[] serials = fields[ nCnt ].GetCustomAttributes( typeof( SerializeField ) , true );
					if( serials.Length == 0 )
					{
						fields.Remove( fields[ nCnt ]);						
					}
				}
				else
				{
					//公有但是添加了[HideInInspector]特性的属性
					object[] hides = fields[ nCnt ].GetCustomAttributes( typeof( HideInInspector ) , true );
					if( hides.Length != 0 )
					{
						fields.Remove( fields[ nCnt ]);						
					}
				}
			}

			//排列
			objectFields_ = fields;

			//AttributesArray初期化
			foreach( var field in objectFields_ )
			{
				if( !attributesArray_.ContainsKey( field.DeclaringType ))
				{
					attributesArray_.Add( field.DeclaringType , new Attributes());
				}
			}
			
			Repaint();
			initialized_ = false;
		} 
	}

	private void OnDisable()
	{
		//解放
		if( attributesArray_ != null )
        {
			foreach( var attribute in attributesArray_ )
			{
				attribute.Value.AllClear();
			}
			attributesArray_.Clear();
		}
	}

	public override void OnInspectorGUI()
	{
		EditorGUI.BeginChangeCheck();
		DrawGUI();
		if( EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();	
	}

	private void HandleProp( SerializedProperty _prop )
	{
		if( _prop.name == "m_Script")
        {
			mSprictProp_ = _prop.Copy();
			return;
        }

		var info = GetFieldInfo( _prop );
		if( info == null ) return;

		Type type = info.DeclaringType;

		if( attributesArray_.ContainsKey( type ))
        {
			bool isSuccess = HandlePropSub( attributesArray_[ type ].categories , _prop );
			if( !isSuccess )
            {
				attributesArray_[ type ].props.Add( _prop.Copy());
			}
		}
        else
        {
			Debug.LogError("dont found this type = " + type.Name );
        }
	}

	private bool HandlePropSub( Dictionary<string, Category> _categories, SerializedProperty _prop )
    {
		if( _categories == null ) return false;

		foreach( var category in _categories )
		{
			if( category.Value.types.Contains( _prop.name ))
			{
				category.Value.props.Add( _prop.Copy());
				return true;
			}
            else
            {
				bool isSuccess = HandlePropSub( category.Value.categories , _prop );
				if( isSuccess )
                {
					return true;
                }
			}
		}
		return false;
	}

	private void DrawGUI()
    {
		//没有序列化内容则不需要自定义渲染		
		if( attributesArray_.Count == 0 )
		{
			DrawDefaultInspector();
			return;
		}

		if( !initialized_ )
		{
			for( var i = 0 ; i < objectFields_.Count ; i++ )
			{
				Type currentType = objectFields_[ i ].DeclaringType;

				var category = Attribute.GetCustomAttribute( objectFields_[ i ] , typeof(CategoryAttribute)) as CategoryAttribute;				
				if( category != null )
				{
					string[] categoryName = category.name_.Split( '|' );
					Category c = null;
					var cArray = attributesArray_[ currentType ].categories;
					for( int j = 0 ; j < categoryName.Length ; j++ )
					{
						if( categoryName[ j ].Length == 0 ) continue;
						if( !cArray.ContainsKey( categoryName[ j ]))
						{
							cArray.Add( categoryName[ j ], new Category{ expanded = category.isFoldout_ ,
																		 useHorizontalLine = category.useHorizontalLine_ ,
																		 titleName = category.titleName_ }); 
						}
						c = cArray[ categoryName[ j ]];
						cArray = c.categories;
					}
					if( c.types == null ) c.types = new HashSet<string>();
					c.types.Add( objectFields_[ i ].Name );
				}                			
			}

			var property = serializedObject.GetIterator();
			var next = property.NextVisible( true );
			if( next )
			{
				do
				{
					HandleProp( property );
				}while( property.NextVisible( false ));
			}
		}

		initialized_ = true;

		using( new EditorGUI.DisabledScope( mSprictProp_ != null ))
		{
			EditorGUILayout.PropertyField( mSprictProp_ , true );
		}

		int count = 0;
		foreach( var attributes in attributesArray_ )
        {
			if( count > 0 ){ EditorGUILayout.Space();}
			EditorGUILayout.LabelField( attributes.Key.Name , scriptTagStyle_ );

			foreach( var prop in attributes.Value.props )
			{
				string tooltip = prop.tooltip != "" ? prop.name + "\n" + prop.tooltip : prop.name;
				EditorGUILayout.PropertyField( prop , new GUIContent( prop.displayName , tooltip ) , true );
			}

			CategoryDraw( attributes.Value.categories );
			count++;
		}
	}

	private void CategoryDraw( Dictionary<string, Category> _categories, int _count = 0 )
    {
		foreach( var category in _categories )
        {
			if( _count == 0 )
			{
				//使用分割线
				if( category.Value.useHorizontalLine )
				{
					EditorGUILayout.Space();
					HorizontalLine.Create( 0 );
					EditorGUILayout.Space();
				}
				//使用标签
				if( category.Value.titleName != null )
				{
					EditorGUILayout.LabelField( category.Value.titleName , categorryTitleStyle_ );
				}
			}

			Rect rect;
			{
				rect = EditorGUILayout.BeginVertical();

				EditorGUI.indentLevel = _count;
				category.Value.expanded = EditorGUILayout.Foldout( category.Value.expanded , category.Key , EditorStyles.foldout );

				EditorGUILayout.EndVertical();
			}

			{
				rect = EditorGUILayout.BeginVertical();

				if( category.Value.expanded )
				{
					for( int i = 0 ; i < category.Value.props.Count ; i++ )
					{
						EditorGUI.indentLevel = _count + 1;
						string tooltip = category.Value.props[ i ].tooltip != "" ? category.Value.props[ i ].name + "\n" + category.Value.props[ i ].tooltip : category.Value.props[ i ].name;
						EditorGUILayout.PropertyField( category.Value.props[ i ], new GUIContent( category.Value.props[ i ].displayName , tooltip ) , true );
					}
				}

				EditorGUI.indentLevel = _count;
				EditorGUILayout.EndVertical();
			}

			if( category.Value.categories != null )
			{
				if( category.Value.expanded )
				{
					CategoryDraw( category.Value.categories , _count + 1 );
				}
			}
		}
	}

	private class Category
	{
		public Dictionary<string, Category> categories = new Dictionary<string, Category>();
		public List<SerializedProperty> props = new List<SerializedProperty>();
		public HashSet<string> types = new HashSet<string>();
		public bool   expanded;
		public bool   useHorizontalLine;
		public string titleName;

		public void Clear()
		{
			categories.Clear();
			props.Clear();
			types.Clear();
		}
	}

	private class Attributes
	{
		public Dictionary<string, Category> categories = new Dictionary<string, Category>();
		public List<SerializedProperty> props = new List<SerializedProperty>();

		public void AllClear()
        {
			AllClearSub(categories);
			categories.Clear();
			props.Clear();
		}

		private void AllClearSub( Dictionary<string, Category> _categories )
        {
			if( _categories == null ) return;
			foreach( var category in _categories )
            {
				AllClearSub( category.Value.categories );
				category.Value.Clear();
			}
        }
	}

	private bool IsTypeCompatible( Type _type )
	{
		if( _type == null || !( _type.IsSubclassOf( typeof(MonoBehaviour)) || _type.IsSubclassOf(typeof(ScriptableObject)))) return false;
		return true;
	}

	private FieldInfo GetFieldInfo( SerializedProperty _property )
	{
		FieldInfo GetField( Type type , string path )
		{
			var info = type.GetField( path , BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly );
			if( info == null && type.BaseType != null )
            {
				return GetField( type.BaseType , path );
            }
			return info;
		}

		var parentType = _property.serializedObject.targetObject.GetType();
		var splits = _property.propertyPath.Split('.');
		var fieldInfo = GetField( parentType , splits[ 0 ]);
		for( var i = 1 ; i < splits.Length ; i++ )
		{
			if( splits[ i ] == "Array" )
			{
				i += 2;
				if( i >= splits.Length ) continue;

				var type = fieldInfo.FieldType.IsArray ? fieldInfo.FieldType.GetElementType() : fieldInfo.FieldType.GetGenericArguments()[ 0 ];
				fieldInfo = GetField( type , splits[ i ]);
			}
			else
			{
				fieldInfo = i + 1 < splits.Length && splits[ i + 1 ] == "Array" ? GetField( parentType , splits[ i ]) : GetField( fieldInfo.FieldType , splits[ i ]);
			}

			if( fieldInfo == null )
			{
				throw new Exception( "Invalid FieldInfo. " + _property.propertyPath );
			}
			parentType = fieldInfo.FieldType;
		}
		return fieldInfo;
	}
}