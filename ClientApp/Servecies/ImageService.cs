using ClientApp.Interfaces;
using ClientApp.Models;
using ClientApp.Requests;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.Servecies
{
    public class ImageService : IImageService
    {
        public JsonFileManager fileManager { get; set; }
        public HttpListenerContext context { get; set; }
        public List<User> registeredUsers { get; set; }

        public ImageService(HttpListenerContext Context, JsonFileManager FileManager, List<User> RegisteredUsers)
        {
            context = Context;
            fileManager = FileManager;
            registeredUsers = RegisteredUsers;
        }
        public async Task Add()
        {
            try
            {
                using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
                string json = await reader.ReadToEndAsync();
                var request = JsonConvert.DeserializeObject<ImageRequest>(json);

                if (request == null || string.IsNullOrWhiteSpace(request.Username) || request.GalleryId <= 0)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Invalid image data or gallery ID"));
                    return;
                }

                var user = registeredUsers.FirstOrDefault(u => u.Username == request.Username);
                if (user == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    await context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("User not found"));
                    return;
                }

                var gallery = user.Galleries.FirstOrDefault(g => g.Id == request.GalleryId);
                if (gallery == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    await context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Gallery not found"));
                    return;
                }

                var imageData = new ImageData
                {
                    Name = request.Name,
                    Description = request.Description,
                    Image = request.Image,
                };

                gallery.Images.Add(imageData);
                fileManager.SaveToFile();

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                await context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Image added successfully"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка добавления изображения: {ex.Message}");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Internal server error"));
            }
        }
        public async Task Update()
        {
            try
            {
                using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
                string json = await reader.ReadToEndAsync();
                var request = JsonConvert.DeserializeObject<ImageEditRequest>(json);

                if (request == null || string.IsNullOrWhiteSpace(request.Username) || request.GalleryId <= 0 || request.Name == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Invalid image data or gallery ID"));
                    return;
                }

                var user = registeredUsers.FirstOrDefault(u => u.Username == request.Username);
                if (user == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    await context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("User not found"));
                    return;
                }

                var gallery = user.Galleries.FirstOrDefault(g => g.Id == request.GalleryId);
                if (gallery == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    await context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Gallery not found"));
                    return;
                }

                var image = gallery.Images.FirstOrDefault(i => i.Id == request.ImageId);
                if (image == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    await context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Image not found"));
                    return;
                }
                image.Name = request.Name ?? image.Name;
                image.Description = request.Description ?? image.Description;
                image.Image = request.Image ?? image.Image;

                fileManager.SaveToFile();

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                await context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Image updated successfully"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка обновления изображения: {ex.Message}");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Internal server error"));
            }
        }
        public async Task Get()
        {
            try
            {
                using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
                string json = await reader.ReadToEndAsync();
                var request = JsonConvert.DeserializeObject<ImageEditRequest>(json);

                if (request == null || string.IsNullOrWhiteSpace(request.Username) || request.GalleryId <= 0 || request.ImageId <= 0)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Invalid image data or gallery ID"));
                    return;
                }

                var user = registeredUsers.FirstOrDefault(u => u.Username == request.Username);
                if (user == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    await context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("User not found"));
                    return;
                }

                var gallery = user.Galleries.FirstOrDefault(g => g.Id == request.GalleryId);
                if (gallery == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    await context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Gallery not found"));
                    return;
                }

                var image = gallery.Images.FirstOrDefault(i => i.Id == request.ImageId);
                if (image == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    await context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Image not found"));
                    return;
                }

                // Return image metadata as a JSON response
                var imageDataResponse = new
                {
                    Name = image.Name,
                    Description = image.Description,
                    Image = image.Image  // Optional: Base64 string of the image itself (if needed)
                };

                string jsonResponse = JsonConvert.SerializeObject(imageDataResponse);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                await context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(jsonResponse));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving image: {ex.Message}");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Internal server error"));
            }
        }
        public async Task Delete()
        {
            try
            {
                using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
                string json = await reader.ReadToEndAsync();
                var request = JsonConvert.DeserializeObject<ImageEditRequest>(json);

                if (request == null || string.IsNullOrWhiteSpace(request.Username) || request.GalleryId <= 0 || request.ImageId <= 0)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Invalid image data or gallery ID"));
                    return;
                }

                var user = registeredUsers.FirstOrDefault(u => u.Username == request.Username);
                if (user == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    await context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("User not found"));
                    return;
                }

                var gallery = user.Galleries.FirstOrDefault(g => g.Id == request.GalleryId);
                if (gallery == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    await context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Gallery not found"));
                    return;
                }

                var image = gallery.Images.FirstOrDefault(i => i.Id == request.ImageId);
                if (image == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    await context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Image not found"));
                    return;
                }

                gallery.Images.Remove(image);
                fileManager.SaveToFile();

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                await context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Image deleted successfully"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка удаления изображения: {ex.Message}");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Internal server error"));
            }
        }
    }
 }
