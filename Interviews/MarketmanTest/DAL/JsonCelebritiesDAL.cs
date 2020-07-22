using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Entities;
using Newtonsoft.Json;

namespace DAL
{
    public class JsonCelebritiesDAL : ICelebritiesDAL
    {
        private static readonly string DB_FOLDER = $"{AppDomain.CurrentDomain.BaseDirectory}\\..\\..\\DataBase\\";

        public List<CelebrityCard> Get() {
            var filePath = $"{DB_FOLDER}Celebrities.json";
            if (!File.Exists(filePath)) return null;
            var sJson = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<IEnumerable<CelebrityCard>>(sJson).ToList();
        }

        public int Save(IEnumerable<CelebrityCard> Cards) {            
            var sJson = JsonConvert.SerializeObject(Cards);
            File.WriteAllText($"{DB_FOLDER}Celebrities.json", sJson); // override
            return Cards.Count();
        }

        public bool Delete(string CardId) {
            var cards = this.Get();
            var originalSize = cards.Count;

            var result = cards.Where(x => x.Id != CardId);                        
            var changes = this.Save(result);
            return changes != originalSize;
        }

        public bool Create(CelebrityCard Card)
        {
            var cards = this.Get();
            var originalSize = cards.Count;

            cards.Add(Card);
            var changes = this.Save(cards);
            return changes != originalSize;
        }

        public bool Update(CelebrityCard Card)
        {
            var cards = this.Get();
            var index = cards.FindIndex(x => x.Id == Card.Id);
            if (index == -1) return false;

            cards[index] = Card;            
            var changes = this.Save(cards);
            return true;
        }
    }
}
