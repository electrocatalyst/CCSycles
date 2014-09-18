/**
Copyright 2014 Robert McNeel and Associates

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
**/

#ifndef __CYCLES__H__
#define __CYCLES__H__

#ifdef CCL_CAPI_DLL
#define CCL_CAPI __declspec (dllexport)
#else
#define CCL_CAPI __declspec (dllimport)
#endif
/*

// conversion matrix for rhino -> cycles view.
ccl::Transform camConvertMat = ccl::make_transform(
1.0f, 0.0f, 0.0f, 0.0f,
0.0f, -1.0f, 0.0f, 0.0f,
0.0f, 0.0f, -1.0f, 1.0f,
0.0f, 0.0f, 0.0f, 1.0f

*/

#ifdef __clplusplus
extern "C" {
#endif

/**
 * \defgroup ccycles CCycles
 * CCycles is the low-level C-API for Cycles. Using this API one can
 * set up the Cycles render engine, push geometry and shaders to it
 * and govern the render process.
 */
/**
 * \defgroup ccycles_scene Scene API
 * The CCycles scene API provides functions to set up a scene for Cycles.
 * A scene will hold object, mesh, light and shader information for the
 * render engine to use during the render process.
 * \ingroup ccycles
 */
/**
 * \defgroup ccycles_shader Shader API
 * The CCycles shader API provides functions to create Cycles
 * shaders giving access to all available shader nodes.
 * \ingroup ccycles
 */
/**
 * \defgroup ccycles_mesh Mesh API
 * The CCycles mesh API provides functions to create Cycles
 * meshes.
 * \ingroup ccycles
 */
/**
 * \defgroup ccycles_object Object API
 * The CCycles object API provides functions to create Cycles
 * objects.
 * \ingroup ccycles
 */
/**
 * \defgroup ccycles_session Session API
 * The CCycles session API provides functions to create Cycles
 * sessions, administer callbacks and manage render processes.
 * \ingroup ccycles
 */

/***********************************/

/**
 * Logger function signature. Used to register a logging
 * function with CCycles using 
 * \ingroup ccycles
 */
typedef void(__cdecl *LOGGER_FUNC_CB)(const char* msg);

/**
 * Status update function signature. Used to register a status
 * update callback function with CCycles using cycles_session_set_update_callback
 * \ingroup ccycles ccycles_session
 */
typedef void(__cdecl *STATUS_UPDATE_CB)(unsigned int session_id);

/**
 * Status update function signature. Used to register a status
 * update callback function with CCycles using cycles_session_set_update_callback
 * \ingroup ccycles ccycles_session
 */
typedef void(__cdecl *RENDER_TILE_CB)(unsigned int session_id, unsigned int x, unsigned int y, unsigned int w, unsigned int h, unsigned int depth);


/**
 * Initialise Cycles by querying available devices.
 * \ingroup ccycles
 */
CCL_CAPI void __cdecl cycles_initialise();

/**
 * Set the path to look for CUDA kernels in.
 * \ingroup ccycles
 */
CCL_CAPI void __cdecl cycles_set_kernel_path(const char* kernel_path);

/**
 * Clean up everything, we're done.
 * \ingroup ccycles
 * \todo Add session specific cleanup, so we don't accidently delete sessions that are in progress.
 */
CCL_CAPI void __cdecl cycles_shutdown();

/**
 * Add a logger function. This will be called only in debug builds.
 * \ingroup ccycles
 */
CCL_CAPI void __cdecl cycles_set_logger(unsigned int client_id, LOGGER_FUNC_CB logger_func_);

/**
 * Set to true if logger output should be sent to std::cout as well.
 *
 * Note that this is global to the logger.
 */
CCL_CAPI void __cdecl cycles_log_to_stdout(int tostdout);
CCL_CAPI unsigned int __cdecl cycles_new_client();
CCL_CAPI void __cdecl cycles_release_client(unsigned int client_id);

/**
 * Query number of available devices.
 * \ingroup ccycles
 */
CCL_CAPI unsigned int __cdecl cycles_number_devices();

/* Query number of available CUDA devices. */
CCL_CAPI unsigned int __cdecl cycles_number_cuda_devices();

/* Query name of a device. */
CCL_CAPI const char* __cdecl cycles_device_description(int i);

/* Query ID of a device. */
CCL_CAPI const char* __cdecl cycles_device_id(int i);

/* Query the index of a device. The index is the nth for the type the device is of. */
CCL_CAPI int __cdecl cycles_device_num(int i);

/* Query if device supports advanced shading. */
CCL_CAPI unsigned int __cdecl cycles_device_advanced_shading(int i);

/* Query if device is used as display device. */
CCL_CAPI unsigned int __cdecl cycles_device_display_device(int i);

/* Query if device supports packing images. */
CCL_CAPI unsigned int __cdecl cycles_device_pack_images(int i);

/** Query device type.
 * \param i device ID.
 * \returns device type
 * \retval 0 None
 * \retval 1 CPU
 * \retval 2 OPENCL
 * \retval 3 CUDA
 * \retval 4 NETWORK
 * \retval 5 MULTI
 */
CCL_CAPI unsigned int __cdecl cycles_device_type(int i);

/* Create scene parameters, to be used when creating a new scene. */
CCL_CAPI unsigned int __cdecl cycles_scene_params_create(unsigned int client_id, unsigned int shadingsystem, unsigned int bvh_type, unsigned int use_bvh_cache, unsigned int use_bvh_spatial_split, unsigned int use_qbvh, unsigned int persistent_data);

/* Set scene parameters*/
CCL_CAPI void __cdecl cycles_scene_params_set_bvh_type(unsigned int client_id, unsigned int scene_params_id, unsigned int type);
CCL_CAPI void __cdecl cycles_scene_params_set_bvh_cache(unsigned int client_id, unsigned int scene_params_id, unsigned int use);
CCL_CAPI void __cdecl cycles_scene_params_set_bvh_spatial_split(unsigned int client_id, unsigned int scene_params_id, unsigned int use);
CCL_CAPI void __cdecl cycles_scene_params_set_qbvh(unsigned int client_id, unsigned int scene_params_id, unsigned int use);
CCL_CAPI void __cdecl cycles_scene_params_set_shadingsystem(unsigned int client_id, unsigned int scene_params_id, unsigned int system);
CCL_CAPI void __cdecl cycles_scene_params_set_persistent_data(unsigned int client_id, unsigned int scene_params_id, unsigned int use);

/**
 * Create a new mesh for object_id in scene_id, using shader_id
 * \ingroup ccycles_scene
 */
CCL_CAPI unsigned int __cdecl cycles_scene_add_mesh(unsigned int client_id, unsigned int scene_id, unsigned int object_id, unsigned int shader_id);
/**
 * Create a new object for scene_id
 * \ingroup ccycles_scene
 */
CCL_CAPI unsigned int __cdecl cycles_scene_add_object(unsigned int client_id, unsigned int scene_id);
/**
 * Set transformation matrix for object
 * \ingroup ccycles_object
 */
CCL_CAPI void __cdecl cycles_scene_object_set_matrix(unsigned int client_id, unsigned int scene_id, unsigned int object_id,
	float a, float b, float c, float d,
	float e, float f, float g, float h,
	float i, float j, float k, float l,
	float m, float n, float o, float p
	);

CCL_CAPI void __cdecl cycles_integrator_set_max_bounce(unsigned int client_id, unsigned int scene_id, int max_bounce);
CCL_CAPI void __cdecl cycles_integrator_set_min_bounce(unsigned int client_id, unsigned int scene_id, int min_bounce);
CCL_CAPI void __cdecl cycles_integrator_set_no_caustics(unsigned int client_id, unsigned int scene_id, bool no_caustics);
CCL_CAPI void __cdecl cycles_integrator_set_transparent_shadows(unsigned int client_id, unsigned int scene_id, bool transparent_shadows);
CCL_CAPI void __cdecl cycles_integrator_set_diffuse_samples(unsigned int client_id, unsigned int scene_id, int diffuse_samples);
CCL_CAPI void __cdecl cycles_integrator_set_glossy_samples(unsigned int client_id, unsigned int scene_id, int glossy_samples);
CCL_CAPI void __cdecl cycles_integrator_set_transmission_samples(unsigned int client_id, unsigned int scene_id, int transmission_samples);
CCL_CAPI void __cdecl cycles_integrator_set_ao_samples(unsigned int client_id, unsigned int scene_id, int ao_samples);
CCL_CAPI void __cdecl cycles_integrator_set_mesh_light_samples(unsigned int client_id, unsigned int scene_id, int mesh_light_samples);
CCL_CAPI void __cdecl cycles_integrator_set_subsurface_samples(unsigned int client_id, unsigned int scene_id, int subsurface_samples);
CCL_CAPI void __cdecl cycles_integrator_set_volume_samples(unsigned int client_id, unsigned int scene_id, int volume_samples);
CCL_CAPI void __cdecl cycles_integrator_set_max_diffuse_bounce(unsigned int client_id, unsigned int scene_id, int max_diffuse_bounce);
CCL_CAPI void __cdecl cycles_integrator_set_max_glossy_bounce(unsigned int client_id, unsigned int scene_id, int max_glossy_bounce);
CCL_CAPI void __cdecl cycles_integrator_set_max_transmission_bounce(unsigned int client_id, unsigned int scene_id, int max_transmission_bounce);
CCL_CAPI void __cdecl cycles_integrator_set_max_volume_bounce(unsigned int client_id, unsigned int scene_id, int max_volume_bounce);
CCL_CAPI void __cdecl cycles_integrator_set_transparent_min_bounce(unsigned int client_id, unsigned int scene_id, int transparent_min_bounce);
CCL_CAPI void __cdecl cycles_integrator_set_transparent_max_bounce(unsigned int client_id, unsigned int scene_id, int transparent_max_bounce);
CCL_CAPI void __cdecl cycles_integrator_set_aa_samples(unsigned int client_id, unsigned int scene_id, int aa_samples);
CCL_CAPI void __cdecl cycles_integrator_set_filter_glossy(unsigned int client_id, unsigned int scene_id, float filter_glossy);
CCL_CAPI void __cdecl cycles_integrator_set_method(unsigned int client_id, unsigned int scene_id, int method);

enum class camera_type : unsigned int {
	PERSPECTIVE = 0,
	ORTHOGRAPHIC,
	PANORAMA
};

enum class panorama_type : unsigned int {
	EQUIRECTANGLUAR = 0,
	FISHEYE_EQUIDISTANT,
	FISHEYE_EQUISOLID
};

CCL_CAPI void __cdecl cycles_camera_set_size(unsigned int client_id, unsigned int scene_id, unsigned int width, unsigned int height);
CCL_CAPI unsigned int __cdecl cycles_camera_get_width(unsigned int client_id, unsigned int scene_id);
CCL_CAPI unsigned int __cdecl cycles_camera_get_height(unsigned int client_id, unsigned int scene_id);
CCL_CAPI void __cdecl cycles_camera_set_type(unsigned int client_id, unsigned int scene_id, camera_type type);
CCL_CAPI void __cdecl cycles_camera_set_panorama_type(unsigned int client_id, unsigned int scene_id, panorama_type type);
CCL_CAPI void __cdecl cycles_camera_set_matrix(unsigned int client_id, unsigned int scene_id,
	float a, float b, float c, float d,
	float e, float f, float g, float h,
	float i, float j, float k, float l,
	float m, float n, float o, float p
	);
CCL_CAPI void __cdecl cycles_camera_compute_auto_viewplane(unsigned int client_id, unsigned int scene_id);
CCL_CAPI void __cdecl cycles_camera_update(unsigned int client_id, unsigned int scene_id);
CCL_CAPI void __cdecl cycles_camera_set_fov(unsigned int client_id, unsigned int scene_id, float fov);
CCL_CAPI void __cdecl cycles_camera_set_sensor_width(unsigned int client_id, unsigned int scene_id, float sensor_width);
CCL_CAPI void __cdecl cycles_camera_set_sensor_height(unsigned int client_id, unsigned int scene_id, float sensor_height);
CCL_CAPI void __cdecl cycles_camera_set_nearclip(unsigned int client_id, unsigned int scene_id, float nearclip);
CCL_CAPI void __cdecl cycles_camera_set_farclip(unsigned int client_id, unsigned int scene_id, float farclip);
CCL_CAPI void __cdecl cycles_camera_set_aperturesize(unsigned int client_id, unsigned int scene_id, float aperturesize);
CCL_CAPI void __cdecl cycles_camera_set_focaldistance(unsigned int client_id, unsigned int scene_id, float focaldistance);
CCL_CAPI void __cdecl cycles_camera_set_shuttertime(unsigned int client_id, unsigned int scene_id, float shuttertime);
CCL_CAPI void __cdecl cycles_camera_set_fisheye_fov(unsigned int client_id, unsigned int scene_id, float fisheye_fov);
CCL_CAPI void __cdecl cycles_camera_set_fisheye_lens(unsigned int client_id, unsigned int scene_id, float fisheye_lens);

/* Create a new session for scene id. */
CCL_CAPI unsigned int __cdecl cycles_session_create(unsigned int client_id, unsigned int session_params_id, unsigned int scene_id);

CCL_CAPI void __cdecl cycles_session_reset(unsigned int client_id, unsigned int session_id, unsigned int width, unsigned int height, unsigned int samples);

CCL_CAPI void __cdecl cycles_session_set_update_callback(unsigned int client_id, unsigned int session_id, void(*update)(unsigned int));
CCL_CAPI void __cdecl cycles_session_set_update_tile_callback(unsigned int client_id, unsigned int session_id, RENDER_TILE_CB update_tile_cb);
CCL_CAPI void __cdecl cycles_session_set_write_tile_callback(unsigned int client_id, unsigned int session_id, RENDER_TILE_CB write_tile_cb);

CCL_CAPI void __cdecl cycles_session_cancel(unsigned int client_id, unsigned int session_id, const char *cancel_message);
CCL_CAPI void __cdecl cycles_session_start(unsigned int client_id, unsigned int session_id);
CCL_CAPI void __cdecl cycles_session_wait(unsigned int client_id, unsigned int session_id);

CCL_CAPI void __cdecl cycles_session_copy_buffer(unsigned int client_id, unsigned int session_id, float* pixel_buffer);
CCL_CAPI void __cdecl cycles_session_get_buffer_info(unsigned int client_id, unsigned int session_id, unsigned int* buffer_size, unsigned int* buffer_stride);

/* session progress access. */
CCL_CAPI int __cdecl cycles_progress_get_sample(unsigned int client_id, unsigned int session_id);
CCL_CAPI void __cdecl cycles_progress_get_tile(unsigned int client_id, unsigned int session_id, int* tile, double* total_time, double* sample_time);
CCL_CAPI void __cdecl cycles_tilemanager_get_sample_info(unsigned int client_id, unsigned int session_id, unsigned int* samples, unsigned int* total_samples);
CCL_CAPI void __cdecl cycles_progress_get_progress(unsigned int client_id, unsigned int session_id, float* progress, double* total_time);
CCL_CAPI const char* __cdecl cycles_progress_get_status(unsigned int client_id, unsigned int session_id);
CCL_CAPI const char* __cdecl cycles_progress_get_substatus(unsigned int client_id, unsigned int session_id);

CCL_CAPI unsigned int __cdecl cycles_session_params_create(unsigned int client_id, unsigned int device);

CCL_CAPI void __cdecl cycles_session_params_set_device(unsigned int client_id, unsigned int session_params_id, unsigned int device);
CCL_CAPI void __cdecl cycles_session_params_set_background(unsigned int client_id, unsigned int session_params_id, unsigned int background);
CCL_CAPI void __cdecl cycles_session_params_set_progressive_refine(unsigned int client_id, unsigned int session_params_id, unsigned int progressive_refine);
CCL_CAPI void __cdecl cycles_session_params_set_output_path(unsigned int client_id, unsigned int session_params_id, const char *output_path);
CCL_CAPI void __cdecl cycles_session_params_set_progressive(unsigned int client_id, unsigned int session_params_id, unsigned int progressive);
CCL_CAPI void __cdecl cycles_session_params_set_experimental(unsigned int client_id, unsigned int session_params_id, unsigned int experimental);
CCL_CAPI void __cdecl cycles_session_params_set_samples(unsigned int client_id, unsigned int session_params_id, int samples);
CCL_CAPI void __cdecl cycles_session_params_set_tile_size(unsigned int client_id, unsigned int session_params_id, unsigned int x, unsigned int y);
CCL_CAPI void __cdecl cycles_session_params_set_tile_order(unsigned int client_id, unsigned int session_params_id, unsigned int tile_order);
CCL_CAPI void __cdecl cycles_session_params_set_start_resolution(unsigned int client_id, unsigned int session_params_id, int start_resolution);
CCL_CAPI void __cdecl cycles_session_params_set_threads(unsigned int client_id, unsigned int session_params_id, unsigned int threads);
CCL_CAPI void __cdecl cycles_session_params_set_display_buffer_linear(unsigned int client_id, unsigned int session_params_id, unsigned int display_buffer_linear);
CCL_CAPI void __cdecl cycles_session_params_set_cancel_timeout(unsigned int client_id, unsigned int session_params_id, double cancel_timeout);
CCL_CAPI void __cdecl cycles_session_params_set_reset_timeout(unsigned int client_id, unsigned int session_params_id, double reset_timeout);
CCL_CAPI void __cdecl cycles_session_params_set_text_timeout(unsigned int client_id, unsigned int session_params_id, double text_timeout);
CCL_CAPI void __cdecl cycles_session_params_set_shadingsystem(unsigned int client_id, unsigned int session_params_id, unsigned int shadingsystem);

/* Create a new scene for specified device. */
CCL_CAPI unsigned int __cdecl cycles_scene_create(unsigned int client_id, unsigned int scene_params_id, unsigned int device_id);
CCL_CAPI void __cdecl cycles_scene_set_background_shader(unsigned int client_id, unsigned int scene_id, unsigned int shader_id);
CCL_CAPI void __cdecl cycles_scene_set_background_ao_factor(unsigned int client_id, unsigned int scene_id, float ao_factor);
CCL_CAPI void __cdecl cycles_scene_set_background_ao_distance(unsigned int client_id, unsigned int scene_id, float ao_distance);
CCL_CAPI void __cdecl cycles_scene_set_background_visibility(unsigned int client_id, unsigned int scene_id, int path_ray_flag);

/* Mesh geometry API */
CCL_CAPI void __cdecl cycles_mesh_set_verts(unsigned int client_id, unsigned int scene_id, unsigned int mesh_id, float *verts, unsigned int vcount);
CCL_CAPI void __cdecl cycles_mesh_set_tris(unsigned int client_id, unsigned int scene_id, unsigned int mesh_id, int *faces, unsigned int fcount, unsigned int shader_id);
CCL_CAPI void __cdecl cycles_mesh_add_triangle(unsigned int client_id, unsigned int scene_id, unsigned int mesh_id, unsigned int v0, unsigned int v1, unsigned int v2, unsigned int shader_id, unsigned int smooth);
CCL_CAPI void __cdecl cycles_mesh_set_uvs(unsigned int client_id, unsigned int scene_id, unsigned int mesh_id, float *uvs, unsigned int uvcount);

/* Shader API */

// NOTE: keep in sync with available Cycles nodes
enum class shadernode_type : unsigned int {
	BACKGROUND = 0,
	OUTPUT,
	DIFFUSE,
	ANISOTROPIC,
	TRANSLUCENT,
	TRANSPARENT,
	VELVET,
	TOON,
	GLOSSY,
	GLASS,
	REFRACTION,
	HAIR,
	EMISSION,
	AMBIENT_OCCLUSION,
	ABSORPTION_VOLUME,
	SCATTER_VOLUME,
	SUBSURFACE_SCATTERING,
	VALUE,
	COLOR,
	MIX_CLOSURE,
	ADD_CLOSURE,
	INVERT,
	MIX,
	GAMMA,
	WAVELENGTH,
	BLACKBODY,
	CAMERA,
	FRESNEL,
	COMBINE_XYZ,
	SEPARATE_XYZ,
	MATH,
	IMAGE_TEXTURE,
	BRICK_TEXTURE,
	SKY_TEXTURE,
	CHECKER_TEXTURE,
	NOISE_TEXTURE,
	WAVE_TEXTURE,
	TEXTURE_COORDINATE
};

CCL_CAPI unsigned int __cdecl cycles_create_shader(unsigned int client_id);
CCL_CAPI unsigned int __cdecl cycles_scene_add_shader(unsigned int client_id, unsigned int scene_id, unsigned int shader_id);
/** Set shader_id as default surface shader for scene_id.
 * Note that shader_id is the ID for the shader specific to this scene.
 * 
 * The correct ID can be found with cycles_scene_shader_id. The ID is also
 * returned from cycles_scene_add_shader.
 */
CCL_CAPI void __cdecl cycles_scene_set_default_surface_shader(unsigned int client_id, unsigned int scene_id, unsigned int shader_id);
/**
 * Return the current default surface shader id for scene_id.
 */
CCL_CAPI unsigned int __cdecl cycles_scene_get_default_surface_shader(unsigned int client_id, unsigned int scene_id);
CCL_CAPI unsigned int __cdecl cycles_scene_shader_id(unsigned int client_id, unsigned int scene_id, unsigned int shader_id);
CCL_CAPI unsigned int __cdecl cycles_add_shader_node(unsigned int client_id, unsigned int shader_id, shadernode_type shn_type);
CCL_CAPI void __cdecl cycles_shadernode_set_attribute_int(unsigned int client_id, unsigned int shader_id, unsigned int shnode_id, const char* attribute_name, int value);
CCL_CAPI void __cdecl cycles_shadernode_set_attribute_float(unsigned int client_id, unsigned int shader_id, unsigned int shnode_id, const char* attribute_name, float value);
CCL_CAPI void __cdecl cycles_shadernode_set_attribute_vec(unsigned int client_id, unsigned int shader_id, unsigned int shnode_id, const char* attribute_name, float x, float y, float z);
CCL_CAPI void __cdecl cycles_shadernode_set_attribute_string(unsigned int client_id, unsigned int shader_id, unsigned int shnode_id, const char* attribute_name, const char* value);
CCL_CAPI void __cdecl cycles_shadernode_set_enum(unsigned int client_id, unsigned int shader_id, unsigned int shnode_id, shadernode_type shn_type, const char* value);

CCL_CAPI void __cdecl cycles_shadernode_set_member_bool(unsigned int client_id, unsigned int shader_id, unsigned int shnode_id, shadernode_type shn_type, const char* member_name, bool value);
CCL_CAPI void __cdecl cycles_shadernode_set_member_float(unsigned int client_id, unsigned int shader_id, unsigned int shnode_id, shadernode_type shn_type, const char* member_name, float value);
CCL_CAPI void __cdecl cycles_shadernode_set_member_int(unsigned int client_id, unsigned int shader_id, unsigned int shnode_id, shadernode_type shn_type, const char* member_name, int value);
CCL_CAPI void __cdecl cycles_shadernode_set_member_vec(unsigned int client_id, unsigned int shader_id, unsigned int shnode_id, shadernode_type shn_type, const char* member_name, float x, float y, float z);

CCL_CAPI void __cdecl cycles_shadernode_set_member_float_img(unsigned int client_id, unsigned int shader_id, unsigned int shnode_id, shadernode_type shn_type, const char* member_name, const char* img_name, float* img, unsigned int width, unsigned int height, unsigned int depth, unsigned int channels);
CCL_CAPI void __cdecl cycles_shadernode_set_member_byte_img(unsigned int client_id, unsigned int shader_id, unsigned int shnode_id, shadernode_type shn_type, const char* member_name, const char* img_name, unsigned char* img, unsigned int width, unsigned int height, unsigned int depth, unsigned int channels);

CCL_CAPI void __cdecl cycles_shader_set_name(unsigned int client_id, unsigned int shader_id, const char* name);
CCL_CAPI void __cdecl cycles_shader_set_use_mis(unsigned int client_id, unsigned int shader_id, unsigned int use_mis);
CCL_CAPI void __cdecl cycles_shader_set_use_transparent_shadow(unsigned int client_id, unsigned int shader_id, unsigned int use_transparent_shadow);
CCL_CAPI void __cdecl cycles_shader_set_heterogeneous_volume(unsigned int client_id, unsigned int shader_id, unsigned int heterogeneous_volume);

CCL_CAPI void __cdecl cycles_shader_connect_nodes(unsigned int client_id, unsigned int shader_id, unsigned int from_id, const std::string from, unsigned int to_id, const std::string to);

/***** LIGHTS ****/
CCL_CAPI unsigned int __cdecl cycles_create_light(unsigned int client_id, unsigned int scene_id, unsigned int light_shader_id);
CCL_CAPI void __cdecl cycles_light_set_type(unsigned int client_id, unsigned int scene_id, unsigned int light_id, unsigned int type);
CCL_CAPI void __cdecl cycles_light_set_spot_angle(unsigned int client_id, unsigned int scene_id, unsigned int light_id, float spot_angle);
CCL_CAPI void __cdecl cycles_light_set_spot_smooth(unsigned int client_id, unsigned int scene_id, unsigned int light_id, float spot_smooth);
CCL_CAPI void __cdecl cycles_light_set_sizeu(unsigned int client_id, unsigned int scene_id, unsigned int light_id, float sizeu);
CCL_CAPI void __cdecl cycles_light_set_sizev(unsigned int client_id, unsigned int scene_id, unsigned int light_id, float sizev);
CCL_CAPI void __cdecl cycles_light_set_axisu(unsigned int client_id, unsigned int scene_id, unsigned int light_id, float axisux, float axisuy, float axisuz);
CCL_CAPI void __cdecl cycles_light_set_axisv(unsigned int client_id, unsigned int scene_id, unsigned int light_id, float axisvx, float axisvy, float axisvz);
CCL_CAPI void __cdecl cycles_light_set_size(unsigned int client_id, unsigned int scene_id, unsigned int light_id, float size);
CCL_CAPI void __cdecl cycles_light_set_dir(unsigned int client_id, unsigned int scene_id, unsigned int light_id, float dirx, float diry, float dirz);
CCL_CAPI void __cdecl cycles_light_set_co(unsigned int client_id, unsigned int scene_id, unsigned int light_id, float cox, float coy, float coz);

#ifdef __clplusplus
}
#endif

#endif