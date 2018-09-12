using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using vega.Controllers.Resources;
using vega.Core;
using vega.Core.Models;

namespace vega.Controllers
{
    [Route("/api/vehicles/{vehicleId}/photos")]
    public class PhotosController : Controller
    {

        private readonly IHostingEnvironment host;
        private readonly IVehicleRepository repository;

        private readonly IPhotoRepository photoRepository;

        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly PhotoSettings photoSettings;

        public PhotosController(IHostingEnvironment host, IVehicleRepository repository, IUnitOfWork unitOfWork, IMapper mapper, IOptionsSnapshot<PhotoSettings> options, IPhotoRepository photoRepository)
        {
            this.photoSettings = options.Value;
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.repository = repository;
            this.host = host;
            this.photoRepository = photoRepository;

        }

        public async Task<IEnumerable<PhotoResource>> GetPhotos(int vehicleId)
        {
            var photos = await photoRepository.GetPhoto(vehicleId);

            return mapper.Map<IEnumerable<Photo>, IEnumerable<PhotoResource>>(photos);
        }

        // /api/vehicles/1/photos
        [HttpPost]
        public async Task<IActionResult> Upload(int vehicleId, IFormFile file)
        {
            var vehicle = await repository.GetVehicle(vehicleId, includeRelated: false); // dont load entire vehicle
            if (vehicle == null)
                return NotFound();

            var uploadFolderPath = Path.Combine(host.WebRootPath, "uploads");
            if (!Directory.Exists(uploadFolderPath))
                Directory.CreateDirectory(uploadFolderPath);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadFolderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            //TODO turn it to thumbnail
            var photo = new Photo { FileName = fileName };
            vehicle.Photos.Add(photo);
            await unitOfWork.CompleteAsync();

            return Ok(mapper.Map<Photo, PhotoResource>(photo));
        }
    }
}