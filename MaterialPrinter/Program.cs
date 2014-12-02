using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ccl;
using ccl.ShaderNodes;
using System.Drawing;
using System.IO;

//using Microsoft.CSharp;
//using System.CodeDom.Compiler;
//using System.Reflection;
//using System.Text;

using System.IO;
using System.Globalization;
using System.CodeDom.Compiler;
using System.Text;
using System.Reflection;
using Microsoft.CSharp;


//using System.Runtime.InteropServices;

namespace MaterialPrinter
{
	public interface IUI
	{
		string getSceneFileName();
		string getMaterialFileName();
		string getOutputFolderName();
		void SetText(string text);
	}

	public class Engine
	{
		/*
		[DllImport("Kernel32.dll")]
		static extern Boolean AllocConsole();

		 * */
		public static class Dynamic_Shader
		{
			private static Client client;
			private static Device device;
			private static Scene scene;

			public static Client Client
			{
				set
				{
					client = Client;
				}
				get
				{
					return client;
				}
			}

			public static Device Device
			{
				set
				{
					device = Device;
				}
				get
				{
					return device;
				}
			}

			public static Scene Scene
			{
				set
				{
					scene = Scene;
				}
				get
				{
					return scene;
				}
			}

			public static Shader Show(Client cl, Device dv, Scene sc, Shader.ShaderType st)
			{
				Client = cl;
				Device = dv;
				Scene = sc;
				
				#region material_hologramu
				var material_hologramu = new Shader(cl, st);

				material_hologramu.Name = "material_hologramu";
				material_hologramu.UseMis = false;
				material_hologramu.UseTransparentShadow = true;
				material_hologramu.HeterogeneousVolume = false;


				var hologram_material_emission = new EmissionNode();
				hologram_material_emission.ins.Strength.Value = 1.000f;

				var image_texture = new ImageTextureNode();
				image_texture.ColorSpace = TextureNode.TextureColorSpace.None;
				image_texture.Projection = TextureNode.TextureProjection.Flat;
				image_texture.Interpolation = InterpolationType.Linear;
				image_texture.Filename = "D:\\Material\\tifs\\TST5.tif";
				using (var bmp = new Bitmap(image_texture.Filename))
				{
					var l = bmp.Width * bmp.Height * 4;
					var bmpdata = new byte[l];
					for (var x = 0; x < bmp.Width; x++)
					{
						for (var y = 0; y < bmp.Height; y++)
						{
							var pos = y * bmp.Width * 4 + x * 4;
							var pixel = bmp.GetPixel(x, y);
							bmpdata[pos] = pixel.R;
							bmpdata[pos + 1] = pixel.G;
							bmpdata[pos + 2] = pixel.B;
							bmpdata[pos + 3] = pixel.A;
						}
					}
					image_texture.ByteImage = bmpdata;
					image_texture.Width = (uint)bmp.Width;
					image_texture.Height = (uint)bmp.Height;
				}

				var input = new TextureCoordinateNode();

				var mapping_001 = new MappingNode();
				mapping_001.vector_type = MappingNode.vector_types.TEXTURE;
				mapping_001.Translation = new float4(1.000f, 1.000f, 1.000f);
				mapping_001.Rotation = new float4(0.000f, 0.000f, 0.000f);
				mapping_001.Scale = new float4(1.000f, 1.000f, 1.000f);

				//var diffuse_bsdf = new DiffuseBsdfNode();
				//diffuse_bsdf.ins.Color.Value = new float4(0.800f, 0.800f, 0.800f);
				//diffuse_bsdf.ins.Roughness.Value = 0.000f;
				//diffuse_bsdf.ins.Normal.Value = new float4(0.000f, 0.000f, 0.000f);


				material_hologramu.AddNode(hologram_material_emission);
				material_hologramu.AddNode(image_texture);
				material_hologramu.AddNode(input);
				material_hologramu.AddNode(mapping_001);
				//material_hologramu.AddNode(output);
				//material_hologramu.AddNode(diffuse_bsdf);

				//hologram_material_emission.outs.Emission.Connect(output.ins.Surface);
				image_texture.outs.Color.Connect(hologram_material_emission.ins.Color);
				input.outs.UV.Connect(mapping_001.ins.Vector);
				mapping_001.outs.Vector.Connect(image_texture.ins.Vector);

				hologram_material_emission.outs.Emission.Connect(material_hologramu.Output.ins.Surface);

				//image_texture.outs.Color.Connect(diffuse_bsdf.ins.Color);


				//diffuse_bsdf.outs.BSDF.Connect(material_hologramu.Output.ins.Surface);

				material_hologramu.FinalizeGraph();

				//sc.AddShader(material_hologramu);
				//sc.Background.Shader = material_hologramu;
				//sc.DefaultSurface = material_hologramu;

				return material_hologramu;
				#endregion
				/*
				
				#region default shader

				var some_setup = new Shader(cl, st)
				{
					Name = "some_setup"
				};

				var brick_texture = new BrickTexture();
				brick_texture.ins.Vector.Value = new float4(0.000f, 0.000f, 0.000f);
				brick_texture.ins.Color1.Value = new float4(0.200f, 0.200f, 0.200f);
				brick_texture.ins.Color2.Value = new float4(0.200f, 0.200f, 0.200f);
				brick_texture.ins.Mortar.Value = new float4(0.000f, 0.000f, 0.000f);
				brick_texture.ins.Scale.Value = 1.000f;
				brick_texture.ins.MortarSize.Value = 0.020f;
				brick_texture.ins.Bias.Value = 0.000f;
				brick_texture.ins.BrickWidth.Value = 0.100f;
				brick_texture.ins.RowHeight.Value = 0.080f;

				var checker_texture = new CheckerTexture();
				checker_texture.ins.Vector.Value = new float4(0.000f, 0.000f, 0.000f);
				checker_texture.ins.Color1.Value = new float4(0.500f, 0.800f, 0.200f);
				checker_texture.ins.Color2.Value = new float4(0.000f, 0.200f, 0.800f);
				checker_texture.ins.Scale.Value = 0.050f;
				
				//var diffuse_bsdf = new DiffuseBsdfNode();
				//diffuse_bsdf.ins.Color.Value = new float4(0.800f, 0.800f, 0.800f);
				//diffuse_bsdf.ins.Roughness.Value = 0.000f;
				//diffuse_bsdf.ins.Normal.Value = new float4(0.000f, 0.000f, 0.000f);
				
				var texture_coordinate = new TextureCoordinateNode();

				some_setup.AddNode(brick_texture);
				some_setup.AddNode(checker_texture);
				//some_setup.AddNode(diffuse_bsdf);
				some_setup.AddNode(texture_coordinate);

				//brick_texture.outs.Color.Connect(diffuse_bsdf.ins.Color);
				checker_texture.outs.Color.Connect(brick_texture.ins.Mortar);
				texture_coordinate.outs.Normal.Connect(checker_texture.ins.Vector);
				texture_coordinate.outs.UV.Connect(brick_texture.ins.Vector);

				//diffuse_bsdf.outs.BSDF.Connect(some_setup.Output.ins.Surface);
				brick_texture.outs.Color.Connect(some_setup.Output.ins.Surface);

				some_setup.FinalizeGraph();

				return some_setup;
				#endregion
				*/
				
			}
		}

		#region meshdata
		static float[] vert_floats =
			{
				-1.0f, -1.0f, 0.0f, -1.0f, 1.0f, 0.0f, 1.0f, 1.0f, 0.0f, 1.0f, -1.0f, 0.0f
				 
			};
		readonly static int[] nverts =
			{
				4
			};
		readonly static int[] vertex_indices =
			{
				0, 2, 1, 0, 3, 2
			};
		#endregion
		private const uint width = 1024;
		private const uint height = 1024;
		private const uint samples = 1;
		static Session Session { get; set; }
		static Client Client { get; set; }
		static Device Device { get; set; }
		static Scene Scene { get; set; }

		private static bool b_init = false;

		public static bool Initialised 
		{              
			set
			{
				b_init = Initialised;
			}
			get
			{
				return b_init;
			}
		}
		
		static IUI _iui;
		public static IUI Iui
		{
			set { Engine._iui = value; }
		}

		public static void ShowMaterial()
		{
			/*
			SetMessage("Creating shader...");
			_iui.RefreshIUI();
			Shader test = Dynamic_Shader.Show(Client, Device, Scene, Shader.ShaderType.World);
			test.FinalizeGraph();
			Scene.DefaultSurface = test;
			Scene.Background.Shader = test;
			SetMessage("Shader \'"+test.Name+"\' Created!!!!");
			 * */
		}

		public static bool CompileMaterial(String sourceName)
		{
			FileInfo sourceFile = new FileInfo(sourceName);
			CodeDomProvider provider = null;
			bool compileOk = false;

			SetMessage("Compiling material: " + sourceName);

			// Select the code provider based on the input file extension.
			if (sourceFile.Extension.ToUpper(CultureInfo.InvariantCulture) == ".CS")
			{
				provider = CodeDomProvider.CreateProvider("CSharp");
			}
			else
			{
				Console.WriteLine("Source file must have a .cs or .vb extension");
			}

			if (provider != null)
			{

				// Format the executable file name.
				// Build the output assembly path using the current directory
				// and <source>_cs.exe or <source>_vb.exe.

				String exeName = String.Format(@"{0}\{1}.exe",
					System.Environment.CurrentDirectory,
					sourceFile.Name.Replace(".", "_"));

				CompilerParameters cp = new CompilerParameters();

				// Generate an executable instead of 
				// a class library.
				cp.GenerateExecutable = false;

				// Specify the assembly file name to generate.
				cp.OutputAssembly = exeName;

				// Save the assembly as a physical file.
				cp.GenerateInMemory = true;

				// Set whether to treat all warnings as errors.
				cp.TreatWarningsAsErrors = false;

				// Add the reference of csycles to the to-be-compiled material
				cp.ReferencedAssemblies.Add("csycles.dll");
				cp.ReferencedAssemblies.Add("System.dll");
				cp.ReferencedAssemblies.Add("System.Drawing.dll");

				// Invoke compilation of the source file.
				CompilerResults cr = provider.CompileAssemblyFromFile(cp, sourceName);

				if (cr.Errors.Count > 0)
				{
					// Display compilation errors.
					SetMessage("Errors building " + sourceName + "into " + cr.PathToAssembly);
					//Console.WriteLine("Errors building {0} into {1}", sourceName, cr.PathToAssembly);
					foreach (CompilerError ce in cr.Errors)
					{
						SetMessage("Error: " + ce.ToString());
						//Console.WriteLine("  {0}", ce.ToString());
						//Console.WriteLine();
					}
				}
				else
				{
					SetMessage("Source " + sourceName + "built into " + cr.PathToAssembly + " succesfully.");
					// Display a successful compilation message.
					//Console.WriteLine("Source {0} built into {1} successfully.", sourceName, cr.PathToAssembly);


					var cls = cr.CompiledAssembly.GetType("HologramPrinter.Dynamic_Shader");
					/*
					var method = cls.GetMethod("CreateDynamicShader", BindingFlags.Static | BindingFlags.Public);   
					var returned_value = method.Invoke(null, null);
					returned_value = Activator.CreateInstance(Type.GetType("ccl.Shader"));
					*/

					Object[] parameters;
					parameters = new Object[4];
					parameters[0] = Client;
					parameters[1] = Device;
					parameters[2] = Scene;
					parameters[3] = Shader.ShaderType.Material;

					MethodInfo methodInformation = cls.GetMethod("Show");
					//object assemblyInstance = cr.CompiledAssembly.CreateInstance("Test Shader", false);
					var returned_value = methodInformation.Invoke(null, parameters);
					SetMessage("Background shader loaded from material file");

					//returned_value = Activator.CreateInstance(Type.GetType("ccl.Shader"));

					if (returned_value is Shader)
					{
						SetMessage("Material loaded into scene!");
						Shader dynamic_shader = returned_value as Shader;
						Scene.AddShader(dynamic_shader);
						Scene.DefaultSurface = dynamic_shader; 
						//Scene.Background.Shader = dynamic_shader;
					}
					else
					{
						SetMessage("Unable to load Material into scene!");
					}



					//var mapping_001 = new MappingNode();
					//mapping_001.outs.Vector.Connect(image_texture.ins.Vector);

					/*
					try
					{
						Shader dynamic_shader = (Shader)Convert.ChangeType(returned_value, typeof(Shader));
						Scene.AddShader(dynamic_shader);
						Scene.Background.Shader = dynamic_shader;
					}
					catch (InvalidCastException)
					{
						Console.WriteLine("Cannot convert an object to a Shader");
					}
					 * */

					/*
					//Shader dynamic_shader = (Shader)returned_value;
					Scene.Background.AoDistance = 0.0f;
					Scene.Background.AoFactor = 0.0f;
					Scene.Background.Visibility = PathRay.PATH_RAY_ALL_VISIBILITY;
					*/

					//String ClassName = "ccl.Shader";
					//Shader dynamic_shader = (Shader)Activator.CreateInstance(cr.CompiledAssembly, ClassName))

					/*
					Shader dynamic_shader = new Shader(Client, Shader.ShaderType.World)
					{
						Name = "Dynamic shader"
					};
					*/

					/*
					Type t = typeof(Shader);
					int val;

					// Get constructor info. 
					ConstructorInfo[] ci = t.GetConstructors();

					Console.WriteLine("Available constructors: ");
					foreach (ConstructorInfo c in ci)
					{
						// Display return type and name. 
						SetMessage("   " + t.Name + "(");

						// Display parameters. 
						ParameterInfo[] pi = c.GetParameters();

						for (int i = 0; i < pi.Length; i++)
						{
							Console.Write(pi[i].ParameterType.Name +
										  " " + pi[i].Name);
							if (i + 1 < pi.Length) Console.Write(", ");
						}

						SetMessage(")");
					}
					SetMessage("");

					// Find matching constructor. 
					int x;

					for (x = 0; x < ci.Length; x++)
					{
						ParameterInfo[] pi = ci[x].GetParameters();
						if (pi.Length == 2) break;
					}

					if (x == ci.Length)
					{
						SetMessage("No matching constructor found.");
					}
					else
						SetMessage("Two-parameter constructor found.\n");

					// Construct the object.   
					object[] consargs = new object[2];
					consargs[0] = Client;
					consargs[1] = Shader.ShaderType.World;
					object reflectOb = ci[x].Invoke(consargs);

					SetMessage("\nInvoking methods on reflectOb.");
					SetMessage("");
					MethodInfo[] mi = t.GetMethods();

					foreach (MethodInfo m in mi)
					{
						 if (m.Name.CompareTo("set") == 0)
						 {

						 }
					}
					 * */

					/*
					// Invoke each method. 
					foreach (MethodInfo m in mi)
					{
						// Get the parameters 
						ParameterInfo[] pi = m.GetParameters();

						if (m.Name.CompareTo("set") == 0 &&
						   pi[0].ParameterType == typeof(int))
						{
							// This is set(int, int). 
							object[] args = new object[2];
							args[0] = 9;
							args[1] = 18;
							m.Invoke(reflectOb, args);
						}
						else if (m.Name.CompareTo("set") == 0 &&
								pi[0].ParameterType == typeof(double))
						{
							// This is set(double, double). 
							object[] args = new object[2];
							args[0] = 1.12;
							args[1] = 23.4;
							m.Invoke(reflectOb, args);
						}
						else if (m.Name.CompareTo("sum") == 0)
						{
							val = (int)m.Invoke(reflectOb, null);
							SetMessage("sum is " + val);
						}
						else if (m.Name.CompareTo("isBetween") == 0)
						{
							object[] args = new object[1];
							args[0] = 14;
							if ((bool)m.Invoke(reflectOb, args))
								SetMessage("14 is between x and y");
						}
						else if (m.Name.CompareTo("show") == 0)
						{
							m.Invoke(reflectOb, null);
						}
					} 
					*/
				}

				// Return the results of the compilation.
				if (cr.Errors.Count > 0)
				{
					compileOk = false;
				}
				else
				{
					compileOk = true;
				}
			}
			return compileOk;
		}
		
		static public void StatusUpdateCallback(uint sessionId)
		{
			float progress;
			double total_time;

			CSycles.progress_get_progress(Client.Id, sessionId, out progress, out total_time);
			var status = CSycles.progress_get_status(Client.Id, sessionId);
			var substatus = CSycles.progress_get_substatus(Client.Id, sessionId);
			uint samples;
			uint num_samples;

			CSycles.tilemanager_get_sample_info(Client.Id, sessionId, out samples, out num_samples);

			if (status.Equals("Finished"))
			{
				Console.WriteLine("wohoo... :D");
			}

			status = "[" + status + "]";
			if (!substatus.Equals(string.Empty)) status = status + ": " + substatus;
			Console.WriteLine("C# status update: {0} {1} {2} {3} <|> {4:N}s {5:P}", CSycles.progress_get_sample(Client.Id, sessionId), status, samples, num_samples, total_time, progress);
		}

		static public void WriteRenderTileCallback(uint sessionId, uint x, uint y, uint w, uint h, uint depth)
		{
			Console.WriteLine("C# Write Render Tile for session {0} at ({1},{2}) [{3}]", sessionId, x, y, depth);
		}

		public static void UpdateRenderTileCallback(uint sessionId, uint x, uint y, uint w, uint h, uint depth)
		{
			Console.WriteLine("C# Update Render Tile for session {0} at ({1},{2}) [{3}]", sessionId, x, y, depth);
		}

		/// <summary>
		/// Callback for debug logging facility. Will be called only for Debug builds of ccycles.dll
		/// </summary>
		/// <param name="msg"></param>
		public static void LoggerCallback(string msg)
		{
			Console.WriteLine("DBG: {0}", msg);
		}

		public static int ColorClamp(int ch)
		{
			if (ch < 0) return 0;
			return ch > 255 ? 255 : ch;
		}

		public static float DegToRad(float ang)
		{
			return ang * (float)Math.PI / 180.0f;
		}

		private static CSycles.UpdateCallback g_update_callback;
		private static CSycles.RenderTileCallback g_update_render_tile_callback;
		private static CSycles.RenderTileCallback g_write_render_tile_callback;

		private static CSycles.LoggerCallback g_logger_callback;

		public static void SetMessage(string msg)
		{
			_iui.SetText(msg);
		}

		public static void Initiate()
		{
			/*
			if (!AllocConsole())
				MessageBox.Show("Failed");
			*/

			SetMessage("Reading input...");

			string materialFile = _iui.getMaterialFileName();//Path.GetFullPath(_iui.getMaterialFileName());
			string sceneFile = _iui.getSceneFileName();//Path.GetFullPath(_iui.getSceneFileName());

			CSycles.set_kernel_path("lib");
			CSycles.initialise();

			SetMessage("Initialising callbacks...");

			g_update_callback = StatusUpdateCallback;
			g_update_render_tile_callback = UpdateRenderTileCallback;
			g_write_render_tile_callback = WriteRenderTileCallback;
			g_logger_callback = LoggerCallback;

			SetMessage("Initialising client...");

			var client = new Client();
			Client = client;
			CSycles.set_logger(client.Id, g_logger_callback);


			SetMessage("Available devices-> Device 0: " + Device.GetDevice(0).Name + ", Device 1: " + Device.GetDevice(1).Name + ", Device 2: " + Device.GetDevice(2).Name + ", Cuda available: " + Device.CudaAvailable() + ", Cuda device: " + Device.FirstCuda.Name);


			SetMessage("Selecting devices...");
			var dev = Device.FirstCuda;
			Device = dev;

			SetMessage("Using device " + dev.Name);

			SetMessage("Creating default scene...");

			#region Create default scene

			var scene_params = new SceneParameters(client, ShadingSystem.SVM, BvhType.Static, true, false, false, false);
			var scene = new Scene(client, scene_params, dev);

			//SetMessage("Setup Camera...");
			/* move the scene camera a bit, so we can see our render result. */
			/*
			var t = Transform.Identity();
			float angle = 90.0f;
			t = t * Transform.Rotate(angle * (float)Math.PI / 180.0f, new float4(0.0f, 1.0f, 0.0f));
			t = t * Transform.Translate(0.0f, 0.0f, 2.188f);
			t = t * Transform.Scale(1.0f, 1.0f, 1.0f);
			*/

			//scene.Camera.Matrix = t;
			/* set also the camera size = render resolution in pixels. Also do some extra settings. */
			//scene.Camera.Size = new Size((int)width, (int)height);
			//scene.Camera.Type = CameraType.Perspective;
			//scene.Camera.ApertureSize = 0.0f;
			//scene.Camera.Fov = 0.661f;
			//scene.Camera.FocalDistance = 0.0f;
			//scene.Camera.SensorWidth = 32.0f;
			//scene.Camera.SensorHeight = 18.0f;
			#endregion

			SetMessage("Creating default shader...");

			//#region default shader

			//var some_setup = new Shader(Client, Shader.ShaderType.Material)
			//{
			//    Name = "some_setup"
			//};

			//var brick_texture = new BrickTexture();
			//brick_texture.ins.Vector.Value = new float4(0.000f, 0.000f, 0.000f);
			//brick_texture.ins.Color1.Value = new float4(0.800f, 0.800f, 0.800f);
			//brick_texture.ins.Color2.Value = new float4(0.200f, 0.200f, 0.200f);
			//brick_texture.ins.Mortar.Value = new float4(0.000f, 0.000f, 0.000f);
			//brick_texture.ins.Scale.Value = 1.000f;
			//brick_texture.ins.MortarSize.Value = 0.020f;
			//brick_texture.ins.Bias.Value = 0.000f;
			//brick_texture.ins.BrickWidth.Value = 0.500f;
			//brick_texture.ins.RowHeight.Value = 0.250f;

			//var checker_texture = new CheckerTexture();
			//checker_texture.ins.Vector.Value = new float4(0.000f, 0.000f, 0.000f);
			//checker_texture.ins.Color1.Value = new float4(0.000f, 0.004f, 0.800f);
			//checker_texture.ins.Color2.Value = new float4(0.200f, 0.000f, 0.007f);
			//checker_texture.ins.Scale.Value = 5.000f;

			//var diffuse_bsdf = new DiffuseBsdfNode();
			//diffuse_bsdf.ins.Color.Value = new float4(0.800f, 0.800f, 0.800f);
			//diffuse_bsdf.ins.Roughness.Value = 0.000f;
			//diffuse_bsdf.ins.Normal.Value = new float4(0.000f, 0.000f, 0.000f);

			//var texture_coordinate = new TextureCoordinateNode();

			//some_setup.AddNode(brick_texture);
			//some_setup.AddNode(checker_texture);
			//some_setup.AddNode(diffuse_bsdf);
			//some_setup.AddNode(texture_coordinate);

			//brick_texture.outs.Color.Connect(diffuse_bsdf.ins.Color);
			//checker_texture.outs.Color.Connect(brick_texture.ins.Mortar);
			//texture_coordinate.outs.Normal.Connect(checker_texture.ins.Vector);
			//texture_coordinate.outs.UV.Connect(brick_texture.ins.Vector);

			//diffuse_bsdf.outs.BSDF.Connect(some_setup.Output.ins.Surface);

			//some_setup.FinalizeGraph();

			//scene.AddShader(some_setup);
			//scene.DefaultSurface = some_setup;
			//#endregion

			SetMessage("Creating shader...");
			Shader test = Dynamic_Shader.Show(Client, Device, Scene, Shader.ShaderType.Material);
			scene.AddShader(test);
			scene.DefaultSurface = test;

			SetMessage("Shader \'" + test.Name + "\' Created!!!!");
			SetMessage("" + test.Type.ToString());

			SetMessage("Creating background shader...");

			#region background shader
			/* Create a simple surface shader and make it the default surface shader */
			var background = new Shader(Client, Shader.ShaderType.World)
			{
				Name = "Background material"
			};

			var bgnode = new BackgroundNode();
			bgnode.ins.Color.Value = new float4(1.0f, 0.64f, 0.0f); //255-165-0
			//bgnode.ins.Strength.Value = 2.0f;

			background.AddNode(bgnode);
			bgnode.outs.Background.Connect(background.Output.ins.Surface);

			background.FinalizeGraph();

			scene.AddShader(background);
			scene.Background.Shader = background;
			scene.Background.Visibility = PathRay.AllVisibility;
			#endregion

			/* get scene-specific default shader ID */
			var default_shader = scene.ShaderSceneId(scene.DefaultSurface);

			//SetMessage("Set integrator settings...");
			/* Set integrator settings */
			/*
			scene.Integrator.IntegratorMethod = IntegratorMethod.Path;
			scene.Integrator.MaxBounce = 1;
			scene.Integrator.MinBounce = 1;
			scene.Integrator.NoCaustics = true;
			scene.Integrator.MaxDiffuseBounce = 1;
			scene.Integrator.MaxGlossyBounce = 1;
			scene.Integrator.Seed = 1;
			scene.Integrator.SamplingPattern = SamplingPattern.Sobol;
			scene.Integrator.FilterGlossy = 0.0f;
			 * */

			SetMessage("Add geometry to scene...");
			/* Add a bit of geometry and move camera around so we can see what we're rendering.
			 * First off we need an object, we put it at the origo
			 */

			var xml = new XmlReader(client, sceneFile);
			xml.Parse();
			var width = (uint)scene.Camera.Size.Width;
			var height = (uint)scene.Camera.Size.Height;


			//var ob = CSycles.scene_add_object(Client.Id, scene.Id);
			//CSycles.object_set_matrix(Client.Id, scene.Id, ob, Transform.Identity());
			//var mesh = CSycles.scene_add_mesh(Client.Id, scene.Id, ob, default_shader);
			
			/* populate mesh with geometry */
			/*
			CSycles.mesh_set_verts(Client.Id, scene.Id, mesh, ref vert_floats, (uint)(vert_floats.Length / 3));
			var index_offset = 0;
			foreach (var face in nverts)
			{
				for (var j = 0; j < face - 2; j++)
				{
					var v0 = (uint)vertex_indices[index_offset];
					var v1 = (uint)vertex_indices[index_offset + j + 1];
					var v2 = (uint)vertex_indices[index_offset + j + 2];

					CSycles.mesh_add_triangle(Client.Id, scene.Id, mesh, v0, v1, v2, default_shader, false);
				}

				index_offset += face;
			}
			*/


			#region point light shader

			var light_shader = new Shader(client, Shader.ShaderType.Material)
			{
				Name = "Tester light shader"
			};

			var emission_node = new EmissionNode();
			emission_node.ins.Color.Value = new float4(0.8f);
			emission_node.ins.Strength.Value = 30.0f;

			light_shader.AddNode(emission_node);
			emission_node.outs.Emission.Connect(light_shader.Output.ins.Surface);
			light_shader.FinalizeGraph();
			scene.AddShader(light_shader);
			#endregion
			
			
			Scene = scene;
						
			CSycles.shutdown();

			Initialised = true;

			//CSycles.shutdown();

			/*

			#region background shader
			var background_shader = new Shader(client, Shader.ShaderType.World)
			{
				Name = "Background shader"
			};

			var bgnode = new BackgroundNode();
			bgnode.ins.Color.Value = new float4(0.0f);
			bgnode.ins.Strength.Value = 1.0f;

			background_shader.AddNode(bgnode);
			bgnode.outs.Background.Connect(background_shader.Output.ins.Surface);
			background_shader.FinalizeGraph();

			scene.AddShader(background_shader);

			scene.Background.Shader = background_shader;
			scene.Background.AoDistance = 0.0f;
			scene.Background.AoFactor = 0.0f;
			scene.Background.Visibility = PathRay.PATH_RAY_ALL_VISIBILITY;
			#endregion
			#region diffuse shader

			var diffuse_shader = create_some_setup_shader();
			scene.AddShader(diffuse_shader);
			scene.DefaultSurface = diffuse_shader;
			#endregion

			

			var xml = new XmlReader(client, sceneFile);
			xml.Parse();
			var width = (uint)scene.Camera.Size.Width;
			var height = (uint)scene.Camera.Size.Height;
			*/

			/* move the scene camera a bit, so we can see our render result. */
			/*
			var t = Transform.Identity();
			t = t * Transform.Rotate(75.0f * (float)Math.PI / 180.0f, new float4(0.0f, 1.0f, 1.0f));
			
			t = t * Transform.Translate(50.0f, 2.0f * (float)tile, -10f);//2.0f * (float)tile
			//scene.Camera.Matrix.x = new float4((float)(2.0f * (float)tile), (float)0.0f, (float)0.0f);

			 * */

			/*
			var transform = Transform.Identity();
			//rotate
			var angle = DegToRad(75.0f);
			var axis = new float4(0.0f, 1.0f, 1.0f);
			transform = transform * ccl.Transform.Rotate(angle, axis);
			//translate
			float tr = 2.0f * (float)tile;
			_iui.SetText("Moving camera: " + tr);
			transform = transform * ccl.Transform.Translate(0.000f, -2.302f+ tr, 0.617f);
			scene.Camera.Matrix = transform;
			*/

		}

		public static void Render(int tile)
		{
			CSycles.set_kernel_path("lib");
			CSycles.initialise();

			/*
			g_update_callback = StatusUpdateCallback;
			g_update_render_tile_callback = UpdateRenderTileCallback;
			g_write_render_tile_callback = WriteRenderTileCallback;
			g_logger_callback = LoggerCallback;
			 * */

			string outFolder = _iui.getOutputFolderName();//Path.GetFullPath(_iui.getOutputFolderName());
			
			_iui.SetText("Setting up session parameters...");

			var session_params = new SessionParameters(Client, Device)
			{
				Experimental = false,
				Samples = (int)samples,
				TileSize = new Size(64, 64),
				StartResolution = 64,
				Threads = 1,
				ShadingSystem = ShadingSystem.SVM,
				Background = true,
				ProgressiveRefine = false
			};
			Session = new Session(Client, session_params, Scene);
			Session.Reset(width, height, samples);

			Session.UpdateCallback = g_update_callback;
			Session.UpdateTileCallback = g_update_render_tile_callback;
			Session.WriteTileCallback = g_write_render_tile_callback;
			
			
			_iui.SetText("Rendering tile " + tile + ": " + CSycles.progress_get_status(Client.Id, Session.Id));

			//_iui.SetText("Devices: " + Device.GetDevice(2).Name);
			//_iui.SetText("Device 0: " + Device.GetDevice(0).Name + ", Device 1: " + Device.GetDevice(1).Name + ", Device 2: " + Device.GetDevice(2).Name + ", Cuda available: " + Device.CudaAvailable() + ", Cuda device: " + Device.FirstCuda.Name);
			
			Session.Start();
			Session.Wait();

			uint bufsize;
			uint bufstride;
			CSycles.session_get_buffer_info(Client.Id, Session.Id, out bufsize, out bufstride);
			var pixels = CSycles.session_copy_buffer(Client.Id, Session.Id, bufsize);

			_iui.SetText("Creating bitmap...");

			var bmp = new Bitmap((int)width, (int)height);
			for (var x = 0; x < width; x++)
			{
				for (var y = 0; y < height; y++)
				{
					var i = y * (int)width * 4 + x * 4;
					var r = ColorClamp((int)(pixels[i] * 255.0f));
					var g = ColorClamp((int)(pixels[i + 1] * 255.0f));
					var b = ColorClamp((int)(pixels[i + 2] * 255.0f));
					var a = ColorClamp((int)(pixels[i + 3] * 255.0f));
					bmp.SetPixel(x, y, Color.FromArgb(a, r, g, b));
				}
			}
			_iui.SetText("Saving bitmap...");

			if (string.IsNullOrEmpty(outFolder))
				bmp.Save("test"+tile+".bmp");
			else
				bmp.Save(outFolder+"\\test"+tile+".bmp");

			_iui.SetText("Done");

			CSycles.shutdown();
		}
	}

	static class Program
	{        
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			//Form form = new Form1();
			Application.Run(new Form1());
		}
	}
}
