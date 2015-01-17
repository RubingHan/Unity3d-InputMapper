//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
// C# example:
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using ws.winx.input;
using ws.winx.platform.windows;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Linq;
using Mono.TextTemplating;
using ws.winx.devices;

namespace ws.winx.editor
{
		public class InputMapper : EditorWindow
		{


	
		
				protected static Dictionary<int,InputState> _stateInputCombinations;// = InputManager.Settings.stateInputs; //new Dictionary<int,InputState> ();
				protected static InputManager.InputSettings settings = InputManager.Settings;
				protected static bool _settingsLoaded = false;
				protected UnityEngine.Object _lastController;
				protected UnityEngine.Object _lastSettingsXML;
				protected static int _selectedStateHash = 0;
				protected static int _deleteStateWithHash = 0;
				protected bool _isDeviceAxisPositionFull;
				protected bool _isComplexActionTypesAllowed;
				protected string _warrningAddStateLabel;
				protected int _isPrimary = 0;
				protected string _currentInputString;

				protected bool _saveBinary=true;


				//Players
				protected int _playerNumber = 1;
				protected int[] _playersIndices;
				protected int _playerIndexSelected = 0;
				protected string[] _playerDisplayOptions;

				//Profiles
			
				int _profileSelectedIndex;
				string[] _profilesDevicesDisplayOptions;
				
				
				//Device
				IDevice _deviceByProfile;
			
				//new state
				protected string _newCustomStateName;
				protected string _prevNewCustomStateName;

		
				//protected string _className="MyEnumClass";
				protected string _namespace = "ws.winx.input.states";
				protected string _enumName = "States";
				protected string _enumFileName = "States.cs";
				protected bool _enumSettingsFoldout;

		
				//styles
				protected GUILayoutOption[] _inputLabelStyle = new GUILayoutOption[]{ GUILayout.Width (200)};
				protected GUILayoutOption[] _stateNameLabelStyle = new GUILayoutOption[]{ GUILayout.Width (100)};
				protected GUILayoutOption[]  _settingsStyle = new GUILayoutOption[]{GUILayout.Width (200)};
				protected GUILayoutOption[]  _addRemoveButtonStyle = new GUILayoutOption[]{GUILayout.Width (40)};
				protected Rect _buttonRect = new Rect (0, 0, 100, 15);
				protected Rect _layerLabelRect = new Rect (0, 0, 100, 15);

				// protected GUILayoutOption[] _warrningAddStateLabelStryle=new GUILayoutOption[]{GUILayout.
				protected InputAction _action;
				protected InputCombination _previousStateInput = null;

				//settings
				protected float _singleClickSensitivity = InputManager.Settings.singleClickSensitivity;
				protected float _doubleClickSensitivity = InputManager.Settings.doubleClickSensitivity;
				protected float _longClickSensitivity = InputManager.Settings.longClickSensitivity;
				protected float _combosClickSensitivity = InputManager.Settings.combinationsClickSensitivity;
				protected string _longClickDesignator = InputManager.Settings.longDesignator;
				protected string _doubleClickDesignator = InputManager.Settings.doubleDesignator;
				protected string _spaceDesignator = InputManager.Settings.spaceDesignator;
				protected char _prevSpaceDesinator = InputManager.Settings.spaceDesignator [0];
				protected string _prevlongClickDesignator = InputManager.Settings.longDesignator;
				protected string _prevdoubleClickDesignator = InputManager.Settings.doubleDesignator;
				protected bool[] _showLayer;
				private static bool __wereDevicesEnumerated = false;


				//PUBLIC
				public Vector2 scrollPosition = Vector2.zero;
				public Vector2 scrollPosition2 = Vector2.zero;
				public int maxCombosNum = 3;
				public UnityEngine.Object settingsFile;
				public AnimatorController controller;
				public bool saveBinary = false;
				public static EditorWindow _instance;
	    
				void Start ()
				{

						Debug.Log ("Start");


//			#if (UNITY_STANDALONE_WIN)
//			InputManager.AddDriver(new ThrustMasterDriver());
//			InputManager.AddDriver(new WiiDriver());
//			InputManager.AddDriver(new XInputDriver());
//			
//			#endif
//			
//			#if (UNITY_STANDALONE_OSX)
//			InputManager.AddDriver(new ThrustMasterDriver());
//			//InputManager.AddDriver(new XInputDriver());
//			#endif
//			
//			#if (UNITY_STANDALONE_ANDROID)
//			InputManager.AddDriver(new ThrustMasterDriver());
//			#endif

						if (!Application.isPlaying) {
								InputManager.hidInterface.Enumerate ();
								__wereDevicesEnumerated = true;
						}
			   
				}

				void Enable ()
				{

						Debug.Log ("Enable");

				}

		


	
				// Add menu named "Input Mapper" to the Window menu
				[MenuItem ("Window/Input Mapper")]
				static void ShowEditor ()
				{
						//if (_stateInputCombinations != null && _stateInputCombinations.Count > 0)
						//		_stateInputCombinations = new Dictionary<int, InputState> ();



						//deselect if some state is selected for editing
						_selectedStateHash = 0;

						// Get existing open window or if none, make a new one:
						if (InputMapper._instance == null)
						if (!Application.isPlaying) {
								InputManager.hidInterface.Enumerate ();
								__wereDevicesEnumerated = true;
						}

						_instance = EditorWindow.GetWindow (typeof(InputMapper));





				}
		
				static void generateCSharpStatesEnum (string namespaceName, string enumName, string fileName, StringBuilder statesStringBuilder)
				{

						//Create template generator
						TemplateGenerator generator = new TemplateGenerator ();
						generator.Session.Add ("Namespace", namespaceName);
						//generator.Session.Add ("ClassName", controller.name.Replace (" ", "") + "Class");
						generator.Session.Add ("EnumName", enumName);		
						generator.Session.Add ("Values", statesStringBuilder.ToString ());
		
						

						string generated;



						generated = Path.Combine (Application.dataPath, "Scripts");
						
						//if Scripts doesn't exist
						if (!Directory.Exists (generated))
								Directory.CreateDirectory (generated);


						//string[] filesFound=Directory.GetFiles (Path.Combine (generated, fileName));

						//if (filesFound.Length == 0) {
						generated = namespaceToDirectory (generated, namespaceName);
						generated = Path.GetFullPath (Path.Combine (generated, fileName));
						//} else {
						//		generated=filesFound[0];
						//}
								

		
						try {

								///!!! OSX : error !Error running gmcs : Cannot find the specified file.! happen here cos of unity bug
								/// Download and install MRE(Mono Runtime Envi)
								/// http://www.mono-project.com/download/
								/// saving doesn't work still cos of unknown
								fileName = Path.Combine (Path.Combine (Application.dataPath, "Editor"), "StatesEnumTemplate.tpl");
								UnityEngine.Debug.Log ("Generating " + generated + " from template:" + fileName);
								generator.ProcessTemplate (fileName, generated);
			
								
						} catch (Exception e) {
								UnityEngine.Debug.LogError (e.Message);
						}
		
						AssetDatabase.Refresh ();

				}


				/// <summary>
				/// Generates Directory Structure based on namespace string ex. ws.winx.input
				/// in specified "rootDir"
				/// </summary>
				/// <returns>The to directory.</returns>
				/// <param name="root">Root.</param>
				/// <param name="namespaceName">Namespace name.</param>
				static string namespaceToDirectory (string root, string namespaceName)
				{

						//split ws.winx.input.states
						string[] namespacePath = namespaceName.Split ('.');
						int i = 0;


						//generate subdirs
						for (; i<namespacePath.Length; i++) {
								root = Path.Combine (root, namespacePath [i]);
								
								if (!Directory.Exists (root))
										Directory.CreateDirectory (root);
						}


						return root;
			
				}
		
				void loadAsset (string path)
				{
						Uri fullPath = new Uri (path, UriKind.Absolute);
						Uri relRoot = new Uri (Application.dataPath, UriKind.Absolute);

						AssetDatabase.ImportAsset (relRoot.MakeRelativeUri (fullPath).ToString (), ImportAssetOptions.ForceUpdate);

				
						_lastSettingsXML = settingsFile = AssetDatabase.LoadAssetAtPath (relRoot.MakeRelativeUri (fullPath).ToString (), typeof(UnityEngine.Object));

						if (_lastSettingsXML != null)
								loadInputSettings (_lastSettingsXML);

						//_lastSettingsXML = settingsXML = AssetDatabase.LoadAssetAtPath (relRoot.MakeRelativeUri(fullPath).ToString(), typeof(TextAsset)) as TextAsset;
						//Debug.Log ("Loading Text asset"+settingsXML.name+" from "+path+" full path:"+ fullPath+" rell:"+relRoot+"relativePath:"+relRoot.MakeRelativeUri(fullPath).ToString());

	
				}
	
	
				///////////////////////////////        LOAD INPUT SETTINGS          //////////////////////
				/// <summary>
				/// Loads the input settings 
				/// </summary>
				void loadInputSettings (UnityEngine.Object asset)
				{
						
						if (asset is TextAsset) {
							
								string text = ((TextAsset)asset).text;

								if (text != null && text.Length > 1) {

										Debug.Log ("Loading..." + _stateInputCombinations);

										//clone
										//_stateInputCombinations = new Dictionary<int,InputState> (_stateInputCombinations);
										//Debug.Log ("Clone..." + _stateInputCombinations.Count);		
						
										//load
										#if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_ANDROID) && !UNITY_WEBPLAYER
											settings=InputManager.loadSettings (new StringReader (text));
										#else
											settings = InputManager.loadSettingsFromText (text, false);
										#endif

										

				
								
								
							
										

										
								
		
								}		

						} else {

								settings = InputManager.loadSettingsFromXMLText (AssetDatabase.GetAssetPath (settingsFile));

						}


						if (settings != null) {

								//assign settings
								_doubleClickDesignator = settings.doubleDesignator;
								_doubleClickSensitivity = settings.doubleClickSensitivity;
								_combosClickSensitivity = settings.combinationsClickSensitivity;
								_singleClickSensitivity = settings.singleClickSensitivity;
								_longClickDesignator = settings.longDesignator;
								_longClickSensitivity = settings.longClickSensitivity;
								_spaceDesignator = settings.spaceDesignator;
				
								_playerNumber = settings.Players.Length;
								_playerIndexSelected = 0;

						}
		    
				}
		


				///////////////////////////////////////  SAVE INPUT SETTINGS /////////////////////////////////////
				/// <summary>
				/// Saves the input settings.
				/// </summary>
				void saveInputSettings (string path)
				{
				

						if (path != null && path.Length > 1) {
								//Debug.Log ("Before..." + _stateInputCombinations.Count+" inputmngr "+InputManager.Settings.stateInputs.Count);
								generateCSharpStatesEnum (_namespace, _enumName, _enumFileName, HashStateInputsToStringBuilder ());
						
			          	
								InputManager.saveSettings (path);

								//Debug.Log ("AfterSave..." + _stateInputCombinations.Count+" inputmngr "+InputManager.Settings.stateInputs.Count+"to path:"+path);
						
										

								loadAsset (path);

								AssetDatabase.Refresh();
						
						}
				}
				


				/// <summary>
				/// Fill StringBuilde with StateName=Hash,...values
				/// </summary>
				/// <returns></returns>
				StringBuilder HashStateInputsToStringBuilder ()
				{
						Dictionary<int,InputState> stateInputsCurrent;
						List<int> inputStatesToBeRemoved;
						List<string> profilesToBeRemoved;
						StringBuilder statesStringBuilder;

						statesStringBuilder = new StringBuilder ();
						
						Dictionary<int,bool> stateInputsCombined = new Dictionary<int, bool> ();
						int numPlayers = settings.Players.Length;

						for (int i=0; i<numPlayers; i++) {

								profilesToBeRemoved = new List<string> ();

								foreach (var DeviceProfileHashStateInput in settings.Players[i].DeviceProfileStateInputs) {


										stateInputsCurrent = DeviceProfileHashStateInput.Value;

										if (stateInputsCurrent.Count == 0) {
												profilesToBeRemoved.Add (DeviceProfileHashStateInput.Key);
												continue;
										}


										inputStatesToBeRemoved = new List<int> ();


							
								
				
										//Filter
										foreach (var HashStateInput in stateInputsCurrent) {


												//fill String Builder with unique StateInputs
												if (!stateInputsCombined.ContainsKey (HashStateInput.Key)) {

														stateInputsCombined [HashStateInput.Key] = true;

														statesStringBuilder.Append ("\t" + HashStateInput.Value.name.Replace (" ", "_") + "=" + HashStateInput.Key + ",\n\r");


												}






												InputCombination combos = HashStateInput.Value.combinations [0];
							
												if (combos != null && combos.GetActionAt (0).codeString == "None") {
														combos.Clear ();
												
														inputStatesToBeRemoved.Add (HashStateInput.Key);
												}
							
							
												combos = HashStateInput.Value.combinations [1];
							
												if (combos != null && combos.GetActionAt (0).codeString == "None") {
														combos.Clear ();
														HashStateInput.Value.combinations [1] = null;
								
												}


										

										
							
					
					
					
										}

										//remove those with "None" as Primary combination
										foreach (var key in inputStatesToBeRemoved) {
												stateInputsCurrent.Remove (key);
										}

								}




									
								//remove empty profiles
								foreach (var key in profilesToBeRemoved) {
										settings.Players [i].DeviceProfileStateInputs.Remove (key);
								}
				
				
						}//end for Players
			
			
			



						return statesStringBuilder;

				}	




				/// <summary>
				/// Replaces the designator.
				/// </summary>
				/// <param name="oldValue">Old value.</param>
				/// <param name="newValue">New value.</param>
				void replaceDesignator (string oldValue, string newValue)
				{
						foreach (var KeyCombinationStringPair in InputMapper._stateInputCombinations) {
								InputCombination combos = KeyCombinationStringPair.Value.combinations [0];

						
								if (combos != null)
										combos.combinationString = combos.combinationString.Replace (oldValue, newValue);

								combos = KeyCombinationStringPair.Value.combinations [1];

								if (combos != null)
										combos.combinationString = combos.combinationString.Replace (oldValue, newValue);

						}
				}




		

				/// <summary>
				/// Exists the in controller.
				/// </summary>
				/// <returns><c>true</c>, if in controller was existed, <c>false</c> otherwise.</returns>
				/// <param name="key">Key.</param>
				bool existInController (int key)
				{


						if (controller != null) {

								int numLayers, numStates, i = 0, j = 0;
								AnimatorControllerLayer layer;
								StateMachine stateMachine;	



								AnimatorController ac = controller as AnimatorController;
								numLayers = ac.layerCount;
			
								for (; i<numLayers; i++) {
										layer = ac.GetLayer (i);
				

										stateMachine = layer.stateMachine;
				
										numStates = stateMachine.stateCount;
				

				
										for (j=0; j<numStates; j++) {

												if (stateMachine.GetState (j).uniqueNameHash == key)
														return true;
						

					
										}
				

				
								}


								return false;
						}


						return false;
				}



				/// <summary>
				/// Resolves empty or None input or restricts input to max num of combinaition(maxCombosNum)
				/// </summary>
				/// <param name="combos">Combos. ex. w+Mouse1(x2)+Joystick1Button3(-)</param>
				/// <param name="input">Input.</param>
				void toInputCombination (InputCombination combos, InputAction input)
				{
				
						if (combos.numActions + 1 > maxCombosNum || (combos.numActions == 1 && combos.GetActionAt (0).getCode (_deviceByProfile) == 0))
								combos.Clear ();
				
						combos.Add (input);

						//Debug.Log(input);

				}


				/////////////////////////                  UPDATE              /////////////////////////
				/// <summary>
				/// Update this instance.
				/// </summary>
				void Update ()
				{
						InputState state;




						if (!Application.isPlaying && _selectedStateHash != 0) {

								if (!__wereDevicesEnumerated) {
										InputManager.hidInterface.Enumerate ();
										__wereDevicesEnumerated = true;
								}

									
								_action = InputManager.GetAction (_deviceByProfile);
				

								if (_action != null && (_action.getCode (_deviceByProfile) ^ (int)KeyCode.Escape) != 0 && (_action.getCode (_deviceByProfile) ^ (int)KeyCode.Return) != 0) {



										if ((_action.getCode (_deviceByProfile) ^ (int)KeyCode.Backspace) == 0) {
												state = _stateInputCombinations [_selectedStateHash];
												state.combinations [_isPrimary].Clear ();
												state.combinations [_isPrimary].Add (new InputAction (KeyCode.None));

										} else {

												
						                        


												if (!_isComplexActionTypesAllowed)
														_action.type = InputActionType.SINGLE;

												_action.setCode (InputCode.toCodeAnyDevice (_action.getCode (_deviceByProfile)), _deviceByProfile);
					

												if (_isDeviceAxisPositionFull) {
														_action.setCode (InputCode.toCodeAxisFull (_action.getCode (_deviceByProfile)), _deviceByProfile);
                                                       
												}

												toInputCombination (_stateInputCombinations [_selectedStateHash].combinations [_isPrimary], _action);
										}



																		//Debug.Log ("Action:" + _action + " " + _action.getCode(_deviceByProfile)+" type:"+_action.type);
								}


								//Debug.Log ("Action:"+action);
						}
               

				

				}



				///////////////////////////             OnGUI                ////////////////////////
				/// <summary>
				/// Raises the GU event.
				/// </summary>
				void OnGUI ()
				{

				
						int numLayers;
						int numStates;
						int i = 0;
						int j = 0;		
						StateMachine stateMachine;
						UnityEditorInternal.State state;
						AnimatorControllerLayer layer;
						AnimatorController ac;


						if (_deleteStateWithHash != 0) {
								_stateInputCombinations.Remove (_deleteStateWithHash);
								_deleteStateWithHash = 0;
						}
					

		
						if (_spaceDesignator != null) {

								EditorGUILayout.LabelField ("Settings");
								EditorGUILayout.Separator ();

								////// MAX NUM COMBOS  ///// 
								maxCombosNum = EditorGUILayout.IntField ("Combos per input:", maxCombosNum, _settingsStyle);

								EditorGUILayout.Separator ();

								////// DESIGNATORS /////
								EditorGUILayout.LabelField ("Click Designators");

								///////// DOUBLE ////////////
								_doubleClickDesignator = EditorGUILayout.TextField ("Double click designator", _doubleClickDesignator, _settingsStyle);
								InputManager.Settings.doubleDesignator = _doubleClickDesignator.Length > 0 ? _doubleClickDesignator : _prevlongClickDesignator;
						
						

								if (_prevdoubleClickDesignator != null && _doubleClickDesignator.Length > 0 && _prevdoubleClickDesignator != _doubleClickDesignator) {
										replaceDesignator (_prevdoubleClickDesignator, _doubleClickDesignator);
								}
								_prevdoubleClickDesignator = _doubleClickDesignator;

								//////////////  LONG //////////
								_longClickDesignator = EditorGUILayout.TextField ("Long click designator", _longClickDesignator, _settingsStyle);

								InputManager.Settings.longDesignator = _longClickDesignator.Length > 0 ? _longClickDesignator : _prevlongClickDesignator;
						
								if (_prevlongClickDesignator != null && _longClickDesignator.Length > 0 && _prevlongClickDesignator != _longClickDesignator) {
										replaceDesignator (_prevlongClickDesignator, _longClickDesignator);
								}
								_prevlongClickDesignator = _longClickDesignator;

								///////////// SPACE /////////////
								_spaceDesignator = EditorGUILayout.TextField ("Combination separator", _spaceDesignator, _settingsStyle);
								if (_spaceDesignator.Length > 1)
										_spaceDesignator = _spaceDesignator [0].ToString ();//restrict to 1 char

								InputManager.Settings.spaceDesignator = _spaceDesignator.Length > 0 ? _spaceDesignator : _prevSpaceDesinator.ToString ();
			
								if (_spaceDesignator.Length > 0 && _prevSpaceDesinator != _spaceDesignator [0]) {
										replaceDesignator (_prevSpaceDesinator.ToString (), _spaceDesignator [0].ToString ());
								}

								_prevSpaceDesinator = _spaceDesignator [0];
						
				
								EditorGUILayout.Separator ();

								/////  SENSITIVITY  ////
								EditorGUILayout.LabelField ("Sensitivity");
								InputManager.Settings.singleClickSensitivity = _singleClickSensitivity = EditorGUILayout.FloatField ("Single click sensitivity", _singleClickSensitivity, _settingsStyle);
								InputManager.Settings.doubleClickSensitivity = _doubleClickSensitivity = EditorGUILayout.FloatField ("Double click sensitivity", _doubleClickSensitivity, _settingsStyle);
								InputManager.Settings.longClickSensitivity = _longClickSensitivity = EditorGUILayout.FloatField ("Long click sensitivity", _longClickSensitivity, _settingsStyle);
								InputManager.Settings.combinationsClickSensitivity = _combosClickSensitivity = EditorGUILayout.FloatField ("Combos click sensitivity", _combosClickSensitivity, _settingsStyle);
								EditorGUILayout.Separator ();


								//////////// PLAYERS //////////////
								EditorGUILayout.BeginHorizontal ();


								InputPlayer player = null;
								_playerNumber = EditorGUILayout.IntField ("Number of Players", _playerNumber);

								if (_playerNumber < 1)
										_playerNumber = 1;
								
							
								//create Players
								if (settings.Players == null || _playerNumber != settings.Players.Length) {
										
										
										InputPlayer[] players = new InputPlayer[_playerNumber];

										for (i=0; i<_playerNumber; i++) {
												
												

												//don't delete previous players just add new
												if (settings.Players != null && settings.Players.Length > i)
														players [i] = settings.Players [i];
												else
														players [i] = new InputPlayer ();
										}


										settings.Players = players;

										//set last player as current
										_playerIndexSelected = _playerNumber - 1;

										//reset profile to default
										_profileSelectedIndex = 0;

										
								}

								//create player display options
								if (_playerDisplayOptions == null || _playerNumber != _playerDisplayOptions.Length) {
										_playerDisplayOptions = new string [_playerNumber];
										for (i=0; i<_playerNumber; i++) {
												_playerDisplayOptions [i] = "Player" + i;
										}

								}




								_playerIndexSelected = EditorGUILayout.Popup (_playerIndexSelected, _playerDisplayOptions);
					

								if (_playerNumber > 1 && GUILayout.Button ("Clone To All")) {
										InputPlayer sample = settings.Players [_playerIndexSelected];
					
										for (i=0; i<_playerNumber; i++) {
												if (i != _playerIndexSelected)
														settings.Players [i] = sample.Clone ();
										}
								}


								EditorGUILayout.EndHorizontal ();




								//////////// Profiles /////////////
				 
								EditorGUILayout.BeginHorizontal ();

								EditorGUILayout.LabelField ("Profiles");



								List<IDevice> devices = InputManager.GetDevices<IDevice> ();

								if (devices.Count > 0) {

										List<string> pList = devices.Where (item => item.profile != null).Select (item => item.profile.Name).Distinct ().ToList ();
										pList.Insert (0, "default");

										_profilesDevicesDisplayOptions = pList.ToArray ();

								} else
										_profilesDevicesDisplayOptions = new string[]{"default"};


				
				
				
				
				
				
								_profileSelectedIndex = EditorGUILayout.Popup (_profileSelectedIndex, _profilesDevicesDisplayOptions);


								//by selecting profile we are setting Device type expectation
								_deviceByProfile = InputManager.GetDevices<IDevice> ().Where (item => item.profile != null).FirstOrDefault (item => item.profile.Name == _profilesDevicesDisplayOptions [_profileSelectedIndex]);

				
				
								player = settings.Players [_playerIndexSelected];

								Dictionary<int,InputState> stateInputsCurrent;

								//init stateInput Dictionary if player numbers is increased
								if (!player.DeviceProfileStateInputs.ContainsKey (_profilesDevicesDisplayOptions [_profileSelectedIndex]))
										player.DeviceProfileStateInputs [_profilesDevicesDisplayOptions [_profileSelectedIndex]] = new Dictionary<int, InputState> ();


								stateInputsCurrent = player.DeviceProfileStateInputs [_profilesDevicesDisplayOptions [_profileSelectedIndex]];


								if (_profileSelectedIndex > 0) {
										if (GUILayout.Button ("Clone default")) {

												Dictionary<int,InputState> stateInputsDefault = player.DeviceProfileStateInputs ["default"];
												foreach (var HashInputStatePair in stateInputsDefault) {
														if (!stateInputsCurrent.ContainsKey (HashInputStatePair.Key))
																stateInputsCurrent.Add (HashInputStatePair.Key, HashInputStatePair.Value.Clone ());
											
												}



										}




						
					

								}


								_stateInputCombinations = stateInputsCurrent;



								EditorGUILayout.EndHorizontal ();
								EditorGUILayout.Separator ();



								

								//////////  ANY/Complex Action Types(doubles,long...)  /FULL AXIS Checkers ///////
								EditorGUILayout.BeginHorizontal ();
								//	_isDeviceAny = GUILayout.Toggle (_isDeviceAny, "Any(Uncheck 4Testing Only");
								_isComplexActionTypesAllowed = GUILayout.Toggle (_isComplexActionTypesAllowed, "Allow DOUBLE/LONG(HOLD)");
								_isDeviceAxisPositionFull = GUILayout.Toggle (_isDeviceAxisPositionFull, "Full Axis");
								EditorGUILayout.EndHorizontal ();

								EditorGUILayout.Separator ();


								


						}
				

						/////////////// GENERATING ENUM SETTINGS  ///////////
						_enumSettingsFoldout = EditorGUILayout.Foldout (_enumSettingsFoldout, "States Enum Properties");

						if (_enumSettingsFoldout) {
								EditorGUILayout.BeginVertical ();

								//settingsXML = EditorGUILayout.ObjectField (settingsXML, typeof(TextAsset), true) as TextAsset;
								_namespace = EditorGUILayout.TextField ("Namespace:", _namespace);
								_enumName = EditorGUILayout.TextField ("Enum:", _enumName);
								_enumFileName = EditorGUILayout.TextField ("File name:", _enumFileName);
								EditorGUILayout.EndVertical ();
						}

						EditorGUILayout.Separator ();

						/////////////////   XML  ////////////////////
						EditorGUILayout.LabelField ("Input XML");
						EditorGUILayout.BeginHorizontal ();
						settingsFile = EditorGUILayout.ObjectField (settingsFile, typeof(UnityEngine.Object), true);

						//reload if xml changed
						if (_lastSettingsXML != settingsFile)
								_settingsLoaded = false;

						_lastSettingsXML = settingsFile;


						if (_selectedStateHash == 0 && GUILayout.Button ("Open")) {
							string path; 

									if(_saveBinary)
									  path=EditorUtility.OpenFilePanel ("Open XML Input Settings file", "", "bin");
									else
										path=EditorUtility.OpenFilePanel ("Open XML Input Settings file", "", "xml");

								if (path.Length > 0) {
										//loadInputSettings (path);

										loadAsset (path);
				
										_settingsLoaded = true;
								}


								return;
						}


						///////////////   SAVE ////////////////////
					
						if (_selectedStateHash == 0 && GUILayout.Button ("Save")) {

								EditorGUIUtility.keyboardControl = 0;
								_selectedStateHash = 0;


								if (!Directory.Exists (Application.streamingAssetsPath)) {
										Directory.CreateDirectory (Application.streamingAssetsPath);
								}

								if (settingsFile != null) {
										if (settingsFile is TextAsset) {
												saveInputSettings (Path.Combine (Application.streamingAssetsPath, settingsFile.name + ".xml"));
										} else {
												saveInputSettings (Path.Combine (Application.streamingAssetsPath, settingsFile.name + ".bin"));
										}
								} else{ 
									    if(_saveBinary)
											saveInputSettings (EditorUtility.SaveFilePanel ("Save Input Settings", Application.streamingAssetsPath, "InputSettings", "bin"));
										else
											saveInputSettings (EditorUtility.SaveFilePanel ("Save Input Settings", Application.streamingAssetsPath, "InputSettings", "xml"));
								return;
								}
						}

			if(settingsFile==null)
				_saveBinary = GUILayout.Toggle (_saveBinary,"Binary");

						/////////// RELOAD ////////////////
						if (GUILayout.Button ("Reload")) { 
								_settingsLoaded = false;
						}
						EditorGUILayout.EndHorizontal ();


						EditorGUILayout.Separator ();

						//loadingSettings 
						if ((!_settingsLoaded && settingsFile != null)) { 
								//loadInputSettings (AssetDatabase.GetAssetPath (settingsXML));
								
								loadInputSettings (settingsFile);
								_settingsLoaded = true;
						}



				
						/////////  ANIMATOR CONTROLER //////////
						_lastController = controller;
			
				
						EditorGUILayout.LabelField ("Animator Controller States");

						EditorGUILayout.BeginHorizontal ();
						controller = EditorGUILayout.ObjectField (controller, typeof(AnimatorController), true) as AnimatorController;

				
						EditorGUILayout.EndHorizontal ();



						EditorGUILayout.Separator ();


				


				
						/////////  Create AnimaitonController states GUI //////////
						if (controller != null) {
		
								ac = controller as AnimatorController;
						

								numLayers = ac.layerCount;

								if (_showLayer == null || _showLayer.Length != numLayers)
										_showLayer = new bool[controller.layerCount];
					   
				
								for (i=0; i<numLayers; i++) {
										layer = ac.GetLayer (i);

								

										_showLayer [i] = EditorGUILayout.Foldout (_showLayer [i], layer.name);

										if (_showLayer [i]) {
												stateMachine = layer.stateMachine;
					
												numStates = stateMachine.stateCount;
					
												scrollPosition = GUILayout.BeginScrollView (scrollPosition, false, false);
				
												for (j=0; j<numStates; j++) {
														state = stateMachine.GetState (j);
						
														createInputStateGUI (state.name, state.uniqueNameHash);
							
												}

												GUILayout.EndScrollView ();
										}

								}


								EditorGUILayout.Separator ();

						}

						///////////////////// NEW CUSTOM STATE STATE //////////////////////
						EditorGUILayout.LabelField ("Custom States");

				
						EditorGUILayout.BeginHorizontal ();
						EditorGUILayout.LabelField ("New state name:", _stateNameLabelStyle);
						_newCustomStateName = EditorGUILayout.TextField (_newCustomStateName, _stateNameLabelStyle);
				
						if (_newCustomStateName != _prevNewCustomStateName)
								_warrningAddStateLabel = "";

						_prevNewCustomStateName = _newCustomStateName;

						if (GUILayout.Button ("+", _addRemoveButtonStyle)) {
			
								EditorGUIUtility.keyboardControl = 0;
								_selectedStateHash = 0;

								if (_newCustomStateName != null && _newCustomStateName.Length > 0) {
										int hash = Animator.StringToHash (_newCustomStateName);
										if (!_stateInputCombinations.ContainsKey (hash)) {//not already there
												_stateInputCombinations [hash] = new InputState (_newCustomStateName, hash);// string[]{"None","None"};
												_stateInputCombinations [hash].Add (new InputCombination ("None"));
												_stateInputCombinations [hash].Add (new InputCombination ("None"));
					
					
												_newCustomStateName = "";//reset
										} else
												_warrningAddStateLabel = "Already exist!";
								} else {

										_warrningAddStateLabel = "Empty state name!";
						
								}
						}


						GUI.color = Color.red;
						EditorGUILayout.LabelField (_warrningAddStateLabel);
						EditorGUILayout.EndHorizontal ();
						GUI.color = Color.white;

				
						EditorGUILayout.BeginHorizontal ();
						scrollPosition2 = EditorGUILayout.BeginScrollView (scrollPosition2, false, false);
						//Debug.Log ("Loop..." + _stateInputCombinations.Count+" inputmngr "+InputManager.Settings.stateInputs.Count);
						if (_stateInputCombinations != null)
								foreach (var KeyValuePair in _stateInputCombinations) {
										if (!existInController (KeyValuePair.Key)) {
												createInputStateGUI (KeyValuePair.Value.name, KeyValuePair.Key);
										}


								}
						GUILayout.EndScrollView ();	
						EditorGUILayout.EndHorizontal ();
		      
						//}




				

				

						//if event is of key or mouse
						if (Event.current.isKey) {

								if (Event.current.keyCode == KeyCode.Return) {
										_selectedStateHash = 0;
										_previousStateInput = null;
										this.Repaint ();
								} else if (Event.current.keyCode == KeyCode.Escape) {
										if (_selectedStateHash != 0) {
												_stateInputCombinations [_selectedStateHash].combinations [_isPrimary] = _previousStateInput;
												_previousStateInput = null;
												_selectedStateHash = 0;
										}
								}		
						}
			
						if (_selectedStateHash != 0)
								InputManager.processGUIEvent (Event.current);//process input from keyboard & mouses
		
		

						

				

				}




				//////////////////////               CREATE STATE GUI             ///////////////////////
			




				/// <summary>
				/// Creates the state GUI.
				/// </summary>
				/// <param name="name">State Name.</param>
				/// <param name="hash">State Hash.</param>
				void createInputStateGUI (string name, int hash)
				{
						InputCombination[] combinations;
						string currentCombinationString;
		
						GUILayout.BeginHorizontal ();
		
		
						GUILayout.Label (name, _stateNameLabelStyle);
		
		
						if (_selectedStateHash != hash) {
			
								if (InputMapper._stateInputCombinations.ContainsKey (hash)) {
				
				
										combinations = InputMapper._stateInputCombinations [hash].combinations;
				
				
										if (combinations [0] == null)
												combinations [0] = new InputCombination ("None");

										if (GUILayout.Button (combinations [0].combinationString)) {
												_selectedStateHash = hash;
												_previousStateInput = null;
												_isPrimary = 0;
												EditorGUIUtility.keyboardControl = 0;
										}
				
										if (combinations [1] == null)
												combinations [1] = new InputCombination ("None");

										if (GUILayout.Button (combinations [1].combinationString)) {
												_selectedStateHash = hash;
												_previousStateInput = null;
												_isPrimary = 1;
												EditorGUIUtility.keyboardControl = 0;
										}


                                  
				
										//DELETE
										if (GUILayout.Button ("-", _addRemoveButtonStyle)) {
												//delete 
												_deleteStateWithHash = hash;
												//this.Repaint();
										}
								} else {
										if (GUILayout.Button ("None")) {

												InputState state = _stateInputCombinations [hash] = new InputState (name, hash);
												state.Add (new InputCombination ((int)KeyCode.None), 0);
						

										
												_selectedStateHash = hash;
												_previousStateInput = null;
												_isPrimary = 0;
												EditorGUIUtility.keyboardControl = 0;
										}
				
				
										if (GUILayout.Button ("None")) {

												InputState state = _stateInputCombinations [hash] = new InputState (name, hash);
												state.Add (new InputCombination ((int)KeyCode.None), 1);
					
										
												_selectedStateHash = hash;
												_previousStateInput = null;
												_isPrimary = 1;
												EditorGUIUtility.keyboardControl = 0;
										}
				
								}
			
			
						} else {

								if (InputMapper._stateInputCombinations.ContainsKey (hash)) {
										combinations = InputMapper._stateInputCombinations [hash].combinations;
			

										currentCombinationString = combinations [_isPrimary].combinationString;

										if (_previousStateInput == null) {
												_previousStateInput = combinations [_isPrimary].Clone ();
										}
						
						
								} else {
										currentCombinationString = "None";
								}


								GUILayout.Label (currentCombinationString);//, _inputLabelStyle);


			
								this.Repaint ();
						}
		
						//			
		
		
						GUILayout.EndHorizontal ();
		
		
		
						EditorGUILayout.Separator ();
				}


            

				///////////////////     ON DESTROY     ////////////////
				void OnDestroy ()
				{

						_selectedStateHash = 0;
						_deleteStateWithHash = 0;

						if (!Application.isPlaying) {

                         

								InputManager.Dispose ();
								
						}
				}
		}
}