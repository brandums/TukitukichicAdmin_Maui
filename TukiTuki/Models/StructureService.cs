using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace TukiTuki.Models
{
    public class StructureService
    {
        private readonly HttpClient _httpClient;
        private readonly string apiUrl = "https://server/api";
        public List<Producto> Productos { get; set; }

        public StructureService()
        {
            _httpClient = new HttpClient();
            GetProductosAsync();
        }

        public async Task<List<Categoria>> GetCategoriasAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{apiUrl}/PrincipalStruct/getCategory");
                response.EnsureSuccessStatusCode();
                var categorias = await response.Content.ReadFromJsonAsync<List<Categoria>>();
                return categorias ?? new List<Categoria>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener categorías: {ex.Message}");
                return new List<Categoria>();
            }
        }

        public async Task<List<Producto>> GetProductosAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{apiUrl}/PrincipalStruct/substruct/1");
                response.EnsureSuccessStatusCode();

                var substruct = await response.Content.ReadFromJsonAsync<Substruct>();

                Productos = new List<Producto>();

                if (substruct?.Nombre != null)
                {
                    for (int i = 0; i < substruct.Nombre.Length; i++)
                    {
                        var producto = new Producto
                        {
                            Codigo = substruct.Codigo[i],
                            Nombre = substruct.Nombre[i],
                            Precio = double.Parse(substruct.Precio?[i] ?? "0"),
                            PrecioDesc = double.Parse(substruct.Extra7?[i] ?? "0"),
                            BreveDescripcion = substruct.BreveDescripcion?[i] ?? string.Empty,
                            Descripcion = substruct.Descripcion?[i] ?? string.Empty,
                            Color = substruct.Color?[i] != null ? substruct.Color[i].ToList() : new List<string>(),
                            Talla = substruct.Talla?[i] != null ? substruct.Talla[i].ToList() : new List<string>(),
                            TipoMedida = substruct.Extra2?[i] ?? string.Empty,
                            Categoria = substruct.Categoria?[i] ?? string.Empty,
                            SubCategoria = substruct.SubCategoria?[i] ?? string.Empty,
                            Stock = int.Parse(substruct.Extra1?[i] ?? "0"),
                            Image = substruct.Images?[i][2],
                            Imagenes = substruct.Images?[i] != null ? new List<string>(substruct.Images[i]) : new List<string>(),
                            Pagina = substruct.Extra4?[i] ?? "",
                            Posicion = substruct.Extra5?[i] ?? "",
                            Estado = substruct.Extra3?[i] ?? "",
                            Promo = substruct.Extra6?[i] ?? "",
                            Descuento = substruct.Extra8?[i] ?? "",
                            VentasBase = substruct.VentasBase?[i],
                            LikesBase = substruct.LikesBase?[i],
                        };

                        Productos.Add(producto);
                    }
                }

                return Productos;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener productos: {ex.Message}");
                return new List<Producto>();
            }
        }

        public async Task<int> GetPaginasAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{apiUrl}/PrincipalStruct/getPages/Todo");
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<int>(jsonResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las páginas: {ex.Message}");
                return 0;
            }
        }

        public async Task ActualizarProductoAsync(Producto producto)
        {
            var content = new StringContent(JsonSerializer.Serialize(producto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{apiUrl}/PrincipalStruct/updateProduct/{producto.Codigo}", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task<bool> UpdateProductPageAsync(string code, int newPage)
        {
            try
            {
                string url = $"{apiUrl}/PrincipalStruct/updateProductPage/{code}/{newPage}";
                var response = await _httpClient.PostAsync(url, null);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating product page: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateProductPositionAsync(string code, int newPosition)
        {
            try
            {
                var response = await _httpClient.PostAsync($"{apiUrl}/PrincipalStruct/updateProductPosition/{code}/{newPosition}", null);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating product position: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateProductStateAsync(int productIndex, bool newState)
        {
            try
            {
                string endpoint = $"{apiUrl}/PrincipalStruct/updateProductState/{productIndex}/{(newState ? "1" : "0")}";
                HttpResponseMessage response = await _httpClient.PostAsync(endpoint, null);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar el estado del producto: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateProductPromoAsync(int index, string newPromo)
        {
            try
            {
                var endpoint = $"{apiUrl}/PrincipalStruct/updateProductPromo/{index}/{newPromo}";
                var response = await _httpClient.PostAsync(endpoint, null);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar la promo: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateProductDiscountAsync(int index, string discount)
        {
            try
            {
                var endpoint = $"{apiUrl}/PrincipalStruct/updateProductDiscount/{index}/{discount}";
                var response = await _httpClient.PostAsync(endpoint, null);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar el descuento: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateVentasBaseAsync(int codigo, int nuevaBase)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{apiUrl}/PrincipalStruct/updateBase/{codigo}/{nuevaBase}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar la base de ventas: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateLikesBaseAsync(int codigo, int nuevaBase)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{apiUrl}/PrincipalStruct/updateLikesBase/{codigo}/{nuevaBase}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar la base de likes: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateTimeOfertaAsync(int codigo, int time)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{apiUrl}/PrincipalStruct/updateTimeOferta/{codigo}/{time}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateSeasonNameAsync(string oldName, string newName)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{apiUrl}/PrincipalStruct/updateCategoryName/{oldName}/{newName}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción en UpdateSeasonNameAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateSubCategoryStatusAsync(string categoryName, string subCategoryName, bool newStatus)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Put, $"{apiUrl}/PrincipalStruct/updateSubCategoryStatus/{categoryName}/{subCategoryName}/{newStatus}");
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var response = await _httpClient.SendAsync(request);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción en UpdateSubCategoryStatusAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Usuario>> GetBotsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{apiUrl}/Opiniones/getBots");
                var usuarios = await response.Content.ReadFromJsonAsync<List<Usuario>>();
                return usuarios;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los bots: {ex.Message}");
                return new List<Usuario>();
            }
        }

        public async Task<bool> CreateUserAsync(string userName)
        {
            try
            {
                var response = await _httpClient.PostAsync($"{apiUrl}/Opiniones/createUser/{userName}", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear el bot: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SaveOpinionAsync(int userId, int productCode, int rating, string comment, string date)
        {
            try
            {
                var response = await _httpClient.PostAsync($"{apiUrl}/Opiniones/BotOpinion/{userId}/{productCode}/{rating}/{comment}/{date}", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar la opinión: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Pedido>> GetPurchasedProductsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{apiUrl}/PrincipalStruct/getPurchasedProducts");
                var result = response.IsSuccessStatusCode;
                var substructs = await response.Content.ReadFromJsonAsync<List<Substruct>>();

                var pedidos = new List<Pedido>();

                foreach (var substruct in substructs)
                {
                    if (int.TryParse(substruct.Extra5[0], out int userId))
                    {
                        var userDetails = new Usuario();

                        var pedido = new Pedido
                        {
                            Usuario = userDetails,
                            Descuento = substruct.Ventas[0],
                            Direccion = substruct.Extra4[0],
                            Pais = substruct.Extra3[0]
                        };

                        pedido.Usuario.Id = int.Parse(substruct.Extra5[0]);
                        pedido.Usuario.Name = substruct.Extra8[0];

                        for (int i = 0; i < substruct.Nombre.Length; i++)
                        {
                            var producto = new Producto
                            {
                                Nombre = substruct.Nombre[i],
                                Cantidad = int.Parse(substruct.Extra1[i]),
                                Precio = double.Parse(substruct.Precio[i]),
                                Color = new List<string>() { substruct.Color[i][0] },
                                Talla = new List<string>() { substruct.Talla[i][0] },
                                Image = substruct.Images[0][0],
                                TipoMedida = substruct.Extra2[i]
                            };
                            pedido.Productos.Add(producto);
                        }

                        if (pedido.Descuento != null && pedido.Descuento != "0")
                        {
                            if (double.TryParse(pedido.Descuento, out double descuento))
                            {
                                pedido.Total = pedido.Productos.Sum(p => p.Precio * p.Cantidad) * (1 - (descuento / 100));
                            }
                            else
                            {
                                pedido.Total = pedido.Productos.Sum(p => p.Precio * p.Cantidad);
                            }
                        }
                        else
                        {
                            pedido.Total = pedido.Productos.Sum(p => p.Precio * p.Cantidad);
                        }

                        pedidos.Add(pedido);
                    }
                }

                return pedidos;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los productos comprados: {ex.Message}");
                return new List<Pedido>();
            }
        }

        public async Task<Usuario> GetUserDetailsAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{apiUrl}/User/{userId}");

                return await response.Content.ReadFromJsonAsync<Usuario>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los detalles del usuario: {ex.Message}");
                return null;
            }
        }

        public async Task<IEnumerable<Usuario>> GetUsersAsync()
        {
            var response = await _httpClient.GetAsync($"{apiUrl}/User");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Usuario>>();
        }

        public async Task<bool> UpdateUserRoleAsync(Usuario usuario, string role)
        {
            try
            {
                if (usuario != null)
                {
                    var response = await _httpClient.GetAsync($"{apiUrl}/User/updateRol/{role}/{usuario.Id}");
                    return response.IsSuccessStatusCode;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar la opinión: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> CreateProduct(Substruct substruct)
        {
            try
            {
                if (substruct != null)
                {
                    var content = new StringContent(JsonSerializer.Serialize(substruct), Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync($"{apiUrl}/PrincipalStruct/addProduct", content);
                    return response.IsSuccessStatusCode;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear el producto: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateProduct(string code, Substruct substruct)
        {
            try
            {
                if (substruct != null)
                {
                    var content = new StringContent(JsonSerializer.Serialize(substruct), Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync($"{apiUrl}/PrincipalStruct/updateProduct/{code}", content);
                    return response.IsSuccessStatusCode;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar el producto: {ex.Message}");
                return false;
            }
        }

        public async Task<string> UploadImageAsync(Stream imageStream, string fileName)
        {
            var content = new MultipartFormDataContent();
            var imageContent = new StreamContent(imageStream);
            imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

            content.Add(imageContent, "image", fileName);

            var response = await _httpClient.PostAsync($"{apiUrl}/PrincipalStruct/UploadImage", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error al subir la imagen: {response.StatusCode}");
            }

            var result = await response.Content.ReadFromJsonAsync<dynamic>();
            return result.GetProperty("imageUrl").GetString();
        }
    }
}
