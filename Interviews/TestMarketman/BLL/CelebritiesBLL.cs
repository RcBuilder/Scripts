using DAL;
using Entities;
using Scrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class CelebritiesBLL : ICelebritiesBLL, ICelebritiesAsyncBLL
    {
        public ICelebritiesDAL DAL { get; set; }
        public IAsyncScrapper<List<CelebrityCard>> Scrapper { get; set; }

        public CelebritiesBLL() : this(new JsonCelebritiesDAL(), new IMDBScrapper(new AgilityPackLoader())) { }
        public CelebritiesBLL(ICelebritiesDAL DAL, IAsyncScrapper<List<CelebrityCard>> Scrapper) {
            this.DAL = DAL;
            this.Scrapper = Scrapper;
        }

        public List<CelebrityCard> Get() {
            var result = this.DAL.Get();
            if (result == null) result = this.Reload();
            return result;
        }

        public int Save(IEnumerable<CelebrityCard> Cards) {
            return this.DAL.Save(Cards);
        }

        public bool Delete(string CardId) {
            return this.DAL.Delete(CardId);
        }

        public bool Create(CelebrityCard Card) {
            return this.DAL.Create(Card);
        }

        public List<CelebrityCard> Reload()
        {
            this.Scrapper.Run();
            this.Save(this.Scrapper.Value);
            return this.Scrapper.Value;
        }  
        
        public bool Update(CelebrityCard Card)
        {
            return this.DAL.Update(Card);            
        }  

        public async Task<List<CelebrityCard>> GetAsync() {
            return await Task.Factory.StartNew(() =>
            {
                return this.Get();
            });
        }

        public async Task<int> SaveAsync(IEnumerable<CelebrityCard> Cards) {
            return await Task.Factory.StartNew(() =>
            {
                return this.Save(Cards);
            });
        }

        public async Task<bool> DeleteAsync(string CardId) {
            return await Task.Factory.StartNew(() =>
            {
                return this.Delete(CardId);
            });
        }

        public async Task<List<CelebrityCard>> ReloadAsync()
        {
            await this.Scrapper.RunAsync();
            await this.SaveAsync(this.Scrapper.Value);
            return this.Scrapper.Value;
        }

        public async Task<bool> CreateAsync(CelebrityCard Card) {
            return await Task.Factory.StartNew(() =>
            {
                return this.Create(Card);
            });
        }

        public async Task<bool> UpdateAsync(CelebrityCard Card)
        {
            return await Task.Factory.StartNew(() =>
            {
                return this.Update(Card);
            });
        }
    }
}
