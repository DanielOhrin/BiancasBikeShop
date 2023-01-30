using BiancasBikeShop.Models;
using BiancasBikeShop.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
namespace BiancasBikeShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BikeController : ControllerBase 
    {
        private IBikeRepository _bikeRepo;

        public BikeController(IBikeRepository bikeRepo)
        {
            _bikeRepo = bikeRepo;
        }

        [HttpGet]
        public IActionResult Get()
        {
            List<Bike> allBikes = _bikeRepo.GetAllBikes();
            return Ok(allBikes);
        }

        [HttpGet("{id}")]
        public IActionResult Get([FromRoute] int id)
        {
            Bike bike = _bikeRepo.GetBikeById(id);

            if (bike == null)
            {
                return NotFound();
            }

            return Ok(bike);
        }

        [HttpGet("in-shop")]
        public IActionResult GetBikesInShopCount()
        {
            int amountInShop = _bikeRepo.GetBikesInShopCount();

            return Ok(amountInShop);
        }
    }
}
