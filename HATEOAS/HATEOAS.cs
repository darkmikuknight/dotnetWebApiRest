using System.Collections.Generic;

namespace dotnetWebApiRest.HATEOAS
{
    public class HATEOAS
    {
        private string url;
        private string protocol = "https://";
        public List<Link> actions = new List<Link>();

        public HATEOAS(string url){
            this.url = url;
        }

        public HATEOAS(string url, string protocol){
            this.url = url;
            this.protocol = protocol;
        }

        public void AddAction(string rel, string method){
            actions.Add(new Link(this.protocol + this.url, rel, method));
        }

        public Link[] GetActions(string sufixo){

            Link[] tempLinks = new Link[actions.Count]; //ToArray(); //ToArray() passa o valor da lista por referência

            for(int i = 0; i< tempLinks.Length; i++){
                tempLinks[i] = new Link(actions[i].href, actions[i].rel, actions[i].method);
            }

            foreach(var link in tempLinks){
                link.href = link.href +"/"+sufixo;
            }

            return tempLinks;
        }
    }
}