using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace HomeRemedies.Data
{
    /// <summary>
    /// Base class for <see cref="SampleDataItem"/> and <see cref="SampleDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class SampleDataCommon : HomeRemedies.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public SampleDataCommon(String uniqueId, String name, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._name = name;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _name = string.Empty;
        public string Name
        {
            get { return this._name; }
            set { this.SetProperty(ref this._name, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(SampleDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return this.Name;
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class SampleDataItem : SampleDataCommon
    {
        public SampleDataItem(String uniqueId, String name, String subtitle, String imagePath, String description, String content, SampleDataGroup group)
            : base(uniqueId, name, subtitle, imagePath, description)
        {
            this._content = content;
            this._group = group;
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        private SampleDataGroup _group;
        public SampleDataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class SampleDataGroup : SampleDataCommon
    {
        public SampleDataGroup(String uniqueId, String name, String subtitle, String imagePath, String description)
            : base(uniqueId, name, subtitle, imagePath, description)
        {
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        if (TopItems.Count > 12)
                        {
                            TopItems.RemoveAt(12);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                    {
                        TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        TopItems.Add(Items[11]);
                    }
                    else if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        TopItems.RemoveAt(12);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        if (Items.Count >= 12)
                        {
                            TopItems.Add(Items[11]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopItems.Clear();
                    while (TopItems.Count < Items.Count && TopItems.Count < 12)
                    {
                        TopItems.Add(Items[TopItems.Count]);
                    }
                    break;
            }
        }

        private ObservableCollection<SampleDataItem> _items = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> Items
        {
            get { return this._items; }
        }

        private ObservableCollection<SampleDataItem> _topItem = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> TopItems
        {
            get { return this._topItem; }
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// 
    /// SampleDataSource initializes with placeholder data rather than live production
    /// data so that sample data is provided at both design-time and run-time.
    /// </summary>
    public sealed class SampleDataSource
    {
        private static SampleDataSource _sampleDataSource = new SampleDataSource();

        private ObservableCollection<SampleDataGroup> _allGroups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<SampleDataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");

            return _sampleDataSource.AllGroups;
        }

        public static SampleDataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static SampleDataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public SampleDataSource()
        {
            String ITEM_CONTENT = String.Format("Item Content: {0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}",
                        "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat vivamus dictumst aliquam duis convallis scelerisque est parturient ullamcorper aliquet fusce suspendisse nunc hac eleifend amet blandit facilisi condimentum commodo scelerisque faucibus aenean ullamcorper ante mauris dignissim consectetuer nullam lorem vestibulum habitant conubia elementum pellentesque morbi facilisis arcu sollicitudin diam cubilia aptent vestibulum auctor eget dapibus pellentesque inceptos leo egestas interdum nulla consectetuer suspendisse adipiscing pellentesque proin lobortis sollicitudin augue elit mus congue fermentum parturient fringilla euismod feugiat");

            var group1 = new SampleDataGroup("acne",
"Disease : Acne",
"Acne is a common human skin disease, characterized by areas of skin with scaly red skin,blackheads and whiteheads, papules, pustules, nodules and possibly scarring ",
"Assets/acne.png",
"Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group1.Items.Add(new SampleDataItem("Group-1-Item-1",
            "Orange",
            "Orange peel is very valuble in treatment of acne",
            "Assets/orange.jpg",
            "",
            "Grind orange peel along with some water to make paste and apply on affected areas.",
            group1));

            group1.Items.Add(new SampleDataItem("Group-1-Item-2", "Almonds",
            "Almond oil is good for treating dry skin and removing scars of old pimples and zits.",
            "Assets/almonds.jpg",
            "",
            "Mix alond powder with a little water and apply the paste on your face. Let it dry for a while and then rinse it off.",
            group1));

            group1.Items.Add(new SampleDataItem("Group-1-Item-3", "Lemon",
            "Lemon performs as an acne remedy since it includes alpha-hydroxy acid, an element in attendance to all citrus fruits.",
            "Assets/Lemon.jpg",
            "",
            "Add one teaspoon lemon juice in one teaspoon finely ground cinnamon powder,mix it and apply on affected areas frequently.",
            group1));

            group1.Items.Add(new SampleDataItem("Group-1-Item-4", "Sandalwood",
            "The most common use of sandalwood can be found as a cure for acne and other skin conditions.",
            "assests/sandalwood.jpg",
            "",
            "Make a paste of one teaspoon each of sandalwood and turmeric powder in a little water and apply on affected areas.",
            group1));

            group1.Items.Add(new SampleDataItem("Group-1-Item-5", "Coriander",
            "Coriander juice, when mixed with a pinch of turmeric powder, serves as an effective remedy against pimples, blackheads and dry skin.",
            "assets/coriander_1.jpg",
            "",
            "Mix a teaspoon of coriander juice with a pinch of turmeric powder and apply this mixture on the face,every night before going to bed.",
            group1));

            group1.Items.Add(new SampleDataItem("Group-1-Item-6", "Cucumber",
            "Cucumber has about 90% water content and serves as a great diuretic. Cucumber for skin works wonders and is considered as a crucial element of skin diet.",
            "assets/Cucumber.jpg",
            "",
            "The healing packs made of grated cucumber,oatmeal cooked in milk and carrots are effective in acne treatment.",
            group1));

            group1.Items.Add(new SampleDataItem("Group-1-Item-7", "Potato",
            "Potato juice will clean the skin, its enzymes acting as a natural antiseptic useful for killing off the bacteria which cause acne outbreaks.",
            "assets/potato.jpg",
            "",
            "Apply the juice of raw potatoes for clearing acne marks. This is a good home remedy for skin blemishes.",
            group1));
            this.AllGroups.Add(group1);
            var group2 = new SampleDataGroup("allergies",
"Disease : Allergies",
"An allergy is a hypersensitivity disorder of the immune system. Allergic reactions occur when a person's immune system reacts to normally harmless substances in the environment. ",

"Assets/allergies-all-off.jpg",
"Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group2.Items.Add(new SampleDataItem("Group-2-Item-1",
            "Castor",
            "Castor oil plays a vital role in treating allergies.",
            "Assets/castoroil.jpg",
            "",
            "Use five drops of castor oil in a little water taken on an empty stomach in morning, is beneficial for allergies in the skin and nasa passages.",
            group2));

            group2.Items.Add(new SampleDataItem("Group-2-Item-2", "Cumin",
            "Cumin soothes the airways, allergic asthma, cough, strengthens the immune system, increases the production of antibodies.",
            "assets/Cumin_seeds.jpg",
            "",
            "Take equal quantity of cumin seeds,fennel seed and white seasame seeds(50grams each) and dry roast these seeds seperately. Mix the roasted seeds and one teaspoon rock salt.Store in a glass vessel. Have this handful of mixture after eating food which will aid digestion and help to prevent any kind of food allergy.",
            group2));

            group2.Items.Add(new SampleDataItem("Group-2-Item-3", "Licorice",
            " Herbalists find that if licorice is used for two weeks before allergy season starts, and is used continously through allergy season, the need of steroids and other prescription drugs can be reduced.",
            "assets/licorice.jpg",
            "",
            "Make licorice tea made from half teaspoon licorice root powder and one teaspoon cow ghee. Add half teaspoon honey only after the tea starts to cool and drink this tea. PLEASE AVOID THIS IF YOU ARE SUFFERING FROM HIGH BLOOD PRESSURE.",
            group2));

            group2.Items.Add(new SampleDataItem("Group-2-Item-4", "General",
            "",
            "assets/general.png",
            "",
            "First try to identify the cause of allergy and avoid them. Work on general health and develop immunity resistance to allergy.",
            group2));
            this.AllGroups.Add(group2);
            var group3 = new SampleDataGroup("anemia",
"Disease : Anemia",
"Anemia is the most common disorder of the blood.Anemia is a decrease in number of red blood cells (RBCs) or less than the normal quantity of hemoglobin in the blood. ",
"Assets/anemia.jpg",
"Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group3.Items.Add(new SampleDataItem("Group-3-Item-1",
            "Apple",
            "Having apple juice can help to cure chronic condition of anemia.",
            "Assets/apple_1.jpg",
            "",
            "Take fresh apple juice an hour before meal or just before going to bed in night. Also do not take anything for about half an hour after the juice.",
            group3));

            group3.Items.Add(new SampleDataItem("Group-3-Item-2", "Beetroot",
           "Beetroot juice is an excellent remedy for anemi. With its high iron contents,beet juice regenerates and reactivates the red blood cells.",
           "assets/beetroot.png",
           "",
           "description",
            group3));

            group3.Items.Add(new SampleDataItem("Group-3-Item-3", "Currants",
            "",
            "Assets/currants.jpg",
            "",
            "Soak 10-15 currants in water overnight. Remove seeds and eat them regularly for one month.",
            group3));

            group3.Items.Add(new SampleDataItem("Group-3-Item-4", "Milk products",
            "Cow's milk is a very good source of protein, calcium and other nutrients that are essential for a child's normal growth and development.",
            "assests/milk products.jpg",
            "",
            "Eat a cup of plain yogurt(curd) with 1 teaspoon turmeric powder on an empty stomach.",
            group3));

            group3.Items.Add(new SampleDataItem("Group-3-Item-5", "Spinach",
            "An important mineral, iron, which is found abundantly in spinach, is known to reproduce red blood cells, thereby preventing chances of anemia.",
            "assets/spinach.jpg",
            "",
            "Have spinach juice of 150gm spinach everyday for one month.",
            group3));

            group3.Items.Add(new SampleDataItem("Group-3-Item-6", "Dates",
            "",
            "assets/dates_1.jpg",
            "",
            "Eat 7-8 dates daily which will help to increase iron level in your blood.",
            group3));

            group3.Items.Add(new SampleDataItem("Group-3-Item-7", "Gooseberry",
            "Gooseberry increases the count of red blood cells.",
            "assets/gooseberry.jpg",
            "",
            "Mix one tablespoon Amla(Indian gooseberry) juice with a ripe mashed banana with one tablespoon honey,take this mixture twice a day.",
            group3));
            this.AllGroups.Add(group3);

            var group4 = new SampleDataGroup("angina",
    "Disease : Angina",
    "Agina is chest pain due to ischemia of heart muscle,generally due to obstruction of the coronary arteries. ",
    "Assets/angina.jpg",
    "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group4.Items.Add(new SampleDataItem("Group-4-Item-1",
            "Almonds",
            "Almonds are the most nutritious nuts because it is high in vitamins and minerals like vitamin E, zinc, calcium, iron, phosphorus, potassium, magnesium, fiber, protein and other essential fatty acids - omega-3 and omega-6.",
            "Assets/almonds.jpg",
            "",
            "Thoroughly mix two teaspoons almond oil with one teaspoon rose oil. Rub gently on the chest two times a day.",
            group4));

            group4.Items.Add(new SampleDataItem("Group-4-Item-2", "Fenugreek",
            "Fenugreek lowers the chloestrol which helps in preventing Angina.",
            "assets/fenugreek_seeds.jpg",
            "",
            "Boil one teaspoon fenugreek seeds in one-and-half cups water. Strain and add two teaspoons honey. Take two times a day.",
            group4));
            this.AllGroups.Add(group4);
            var group5 = new SampleDataGroup("arthritis",
    "Disease : Arthritis",
      "Arthritis is a form of joint disorder that involves inflammation of one or more joints. ",
      "Assets/arthritis.jpg",
    "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group5.Items.Add(new SampleDataItem("Group-5-Item-1",
            "Castor",
            "Castor oil contains natural steroids, which help to heal the inflamatory conition of rheumatoid arthritis.",
            "Assets/castoroil.jpg",
            "",
            "Drink one cup of ginger tea with two teaspoons of castor oil added before going to sleep in night. ",
            group5));

            group5.Items.Add(new SampleDataItem("Group-5-Item-2", "Seasame",
            "Seasame is very effective in preventing frequent joint pains.",
            "Assets/seasame.jpg",
            "",
            "Soak 1 teaspoon of black seasame seeds in a half cup of water overnight. Take these seeds along with water first thing in the morning.",
            group5));

            group5.Items.Add(new SampleDataItem("Group-5-Item-3", "Garlic",
            "Garlic has anti-inflammatory properties,effective in the treatment of arthritis.",
            "Assets/garlic.jpg",
            "",
            "Eat 2-3 cloves of garlic everyday.",
            group5));

            group5.Items.Add(new SampleDataItem("Group-5-Item-4", "Ginger",
            "Ginger contains effective anti inflammatory properties with the help of which you can heal your aching joints.",
            "assests/ginger.jpg",
             "",
            "Apply ginger compresses on the affected areas which will ease out the pain.",
            group5));

            group5.Items.Add(new SampleDataItem("Group-5-Item-5", "Potato",
            "The raw potato juice is considered one of the most effective treatment for rheumatoid and arthritis pain.",
            "assets/potato.jpg",
            "",
            "Put the thin potato slices without peeling the skin overnight in a large glass filled with water and drink in the morning on an empty stomach.",
            group5));

            group5.Items.Add(new SampleDataItem("Group-5-Item-6", "Walnut",
            "Wslnuts are rich in omega-3 fatty acids which help in the state of arthritis.",
            "assets/walnuts.jpg",
            "",
            "On an empty stomach eat 3-4 walnuts in the morning.",
            group5));

            group5.Items.Add(new SampleDataItem("Group-5-Item-7", "Pineapple",
            "Fresh pineapple juice reduces swelling and inflammation in osteoarthritis and rheumatoid arthritis.",
            "assets/pineapple.jpg",
            "",
            "Drinking a cup of fresh pineapple juice which is good. ",
            group5));
            this.AllGroups.Add(group5);
            var group6 = new SampleDataGroup("asthma",
    "Disease : Asthma",
    "Asthma is comman chronic inflammatory disease of the airways characterized by variable and recurring reversible airflow obstruction. ",
    "Assets/asthma.jpg",
    "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group6.Items.Add(new SampleDataItem("Group-6-Item-1",
            "Mustard",
            "Healing the bronchial system with mustard seeds can be most effective.",
            "Assets/mustard.jpg",
            "",
            "During the attack massage your chest with mustard oil mixed with little camphor. This will loosen up phlegm and ease out breathing. Also inhale steam from boiling water mixed with caraway seeds(ajwain).It helps to distend the bronchial passage.",
            group6));

            group6.Items.Add(new SampleDataItem("Group-6-Item-2", "General",
            "",
            "Assets/general.png",
            "",
            "Take steam bath and sun bath.This will stimulate the skin and relieve congested chest. Apply mud-packs to abdomen which will relieve the fermentation caused by undigested food.",
            group6));

            group6.Items.Add(new SampleDataItem("Group-6-Item-3", "Onion",
            "Onions contain a variety of anti-inflammatory compounds that may improve symptoms of asthma.",
            "Assets/onion.jpg",
            "",
            "Take 1/4 cup of onion juice,add one teaspoon honey and pinch of black pepper and drink,which will relieve congestion and is effective for immediate relief from asthma.",
            group6));

            group6.Items.Add(new SampleDataItem("Group-6-Item-4", "Honey",
            "Honey is considered highly beneficial for treatment of asthma.",
            "assests/honey_1.jpg",
            "",
            "Eat honey or take either in milk or water. It clears the respiratory passages. One year old honey is recommended for respiratory diseases.",
            group6));

            group6.Items.Add(new SampleDataItem("Group-6-Item-5", "Garlic",
            "Garlic is another effective home remedy for asthma.",
            "assets/garlic.jpg",
            "",
            "Take garlic cloves boiled in 30gms of milk daily as a cur for early stage of asthma.",
            group6));

            group6.Items.Add(new SampleDataItem("Group-6-Item-6", "Turmeric",
            "Turmeric is an effective remedy for bronchial asthma.",
            "assets/turmeric.jpg",
            "",
            "Take a teaspoon of turmeric powder boiled in a glass of milk,two or three times daily.Best is to take it on an empty stomach.",
            group6));

            group6.Items.Add(new SampleDataItem("Group-6-Item-7", "Carrot",
            "Carrots benefit the lungs and are phlegm resolving and have been found to be an effective treatment for many with asthma.",
            "assets/carrot1.jpg",
            "",
            "Take half glass of carrot juice and quater glass of spinach juice thrice in a day.",
            group6));
            this.AllGroups.Add(group6);
            var group7 = new SampleDataGroup("athlete's foot",
"Disease : Athlete's foot",
"Athlete's foot is a fungal infection of the skin that causes scaling,flaking and itch of the affected areas ",
"assets/athlete's foot.jpg",
"Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group7.Items.Add(new SampleDataItem("Group-7-Item-1",
            "Baking soda",
            "Baking soda can work wonders for a lot of different ailments.",
            "Assets/baking soda.jpg",
            "",
            "Baking soda paste can be used between toes if the fungus is there.For prearing paste,add a few drops of water to one tablespoon of baking soda. Rub it all over the affected area,rinse it off and powder your feet with corn starch.",
            group7));

            group7.Items.Add(new SampleDataItem("Group-7-Item-2", "Vinegar",
            " Vinegar has antifungal properties that kills the fungus and prevents athletes foot from returning.",
            "assets/vinegar.jpg",
            "",
            "Soak feet in a very warm solution of one litre of white vinegar to four litre of water for half an hour twice a day.",
            group7));
            this.AllGroups.Add(group7);
            var group8 = new SampleDataGroup("back pain",
    "Disease : Back pain",
    "Back pain is pain felt in back that usually originates from the muscles,nerves,bones,joints or other structures in the spine. ",
    "Assets/back pain.jpg",
    "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group8.Items.Add(new SampleDataItem("Group-8-Item-1",
            "General",
            "",
            "Assets/general.png",
            "",
            "Apply an ice pack to the painful area for 15 minutes. Apply at least three times a day.",
            group8));

            group8.Items.Add(new SampleDataItem("Group-8-Item-2", "Garlic",
            "Garlic has been found to be one of the most effective remedies for the treatment of back pain. It can either be eaten or its oil can be prepared from the garlic cloves for topical use. It also helps in reducing swelling and pain because of its anti-inflammatory properties.",
            "assets/garlic.jpg",
            "",
            "Take two or three cloves of garlic every morning. Rubbing the painful area of the back with garlic oil is also good for relieving the pain. Prepare the oil by frying 10 cloves of garlic in 60ml of mustard oil or seasame oil on a slow fire till the garlic cloves turn brown. Then,the cooked oil should be applied on the back. Take a warm bath after 2-3 hours. Follow this for atleast 15days.",
            group8));

            group8.Items.Add(new SampleDataItem("Group-8-Item-3", "Ginger",
            "Ginger contains anti-inflammatory compounds, including some with mild aspirin-like effects.",
            "assets/ginger.jpg",
            "",
            "Apply a paste made of ginger powder mixed with sufficient water to the affected area. Leave it on for 15minutes,wash it off and then rub the back with eucalyptus oil. Apply a paste of ginger and eucalyptus oil to the affected area.",
            group8));

            group8.Items.Add(new SampleDataItem("Group-8-Item-4", "Potato",
            "The raw potato poultice can be used as a natural treatment for low back pain, as it is thought to reduce swelling, relieve pain, promote absorption of nutrients, encourage muscle relaxation and support a healthy inflammatory response to injury.",
            "assets/potato.jpg",
            "",
            "Application of raw potato in the form of a poultice is very effective in the back pain,especially in a lower back.",
            group8));

            group8.Items.Add(new SampleDataItem("Group-8-Item-5", "Lemon",
            "Comsumption of lemon juice is very effective in back pain.",
            "assets/Lemon.jpg",
            "brief description2",
            "Take the juice of one lemon mixed with pinch of comman salt twice a day.",
            group8));
            this.AllGroups.Add(group8);
            var group9 = new SampleDataGroup("bad breath",
    "Disease : Bad breath",
    "Bad breath occurs when noticeably unpleasant odors are exhaled in breathing. ",
    "Assets/bad breath.jpg",
    "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group9.Items.Add(new SampleDataItem("Group-9-Item-1",
            "Lemon",
            "Lemon contains citric acid, it quickly changes the body's acid condition into an alkaline one. Lemons also contain phosphorus.",
            "Assets/Lemon.jpg",
            "",
            "Rinse mouth with lemon juice mixed in water.",
            group9));

            group9.Items.Add(new SampleDataItem("Group-9-Item-2", "Cardamom",
            "Cardamom for bad breath is known to be very effective as it tends to neutralize the dental bacteria responsible for bad breath.",
            "assets/cardamom.jpg",
            "",
            "Slowly chewing one or two cardamom seeds also helps to minimize bad breath.",
            group9));

            group9.Items.Add(new SampleDataItem("Group-9-Item-3", "Fenugreek",
            "Fenugreek is the most popular and effective home remedy for bad breath.",
            "assets/fenugreek-seeds.jpg",
            "",
            "Atea made from the seeds of fenugreek is useful. Use one teaspoon of seeds in half a litre of water and allow it to boil slowly for 20minutes. It should then be strained and used as tea. ",
            group9));

            group9.Items.Add(new SampleDataItem("Group-9-Item-4", "Aloe vera",
            "Using Aloe can help for bad breath. It is a natural anti-fungal and antibacterial which helps against bad breath.",
            "assets/aloe vera.jpg",
            "",
            "Drink half cup aloe vera juice twice in a day.",
            group9));
            this.AllGroups.Add(group9);
            var group10 = new SampleDataGroup("baldness",
    "Disease : Baldness",
    "Baldness is partial or complete lack of hair. ",
    "Assets/baldness.jpg",
    "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group10.Items.Add(new SampleDataItem("Group-10-Item-1",
            "Fenugreek",
            "Fenugreek sprouts promote hair growth.",
            "Assets/fenugreek-seeds.jpg",
            "",
            "Grind fenugreek seeds with water and apply on the head. Leave it for atleast 40minutes and wash it. Do it every morning for 30days.",
            group10));

            group10.Items.Add(new SampleDataItem("Group-10-Item-2", "Seasame",
            "This highly nourishing hair oil has immense healing properties. Its use has been extensively mentioned in ancient Ayurvedic text for over thousands of years.",
                        "Assets/seasame.jpg",
                        "",
                        "Eat handful of white seasame seeds in the morning.",
            group10));

            group10.Items.Add(new SampleDataItem("Group-10-Item-3", "Lemon",
            "Lemon provides many benefits for skin, hair and the digestive system and is rich in vitamin C and citric acid.",
            "Assets/Lemon.jpg",
            "",
            "Apply paste made from the seeds of lemon and black pepper in water to affected areas,wash with water if it causes skin irritation. This paste stimulates hair growth.",
            group10));

            group10.Items.Add(new SampleDataItem("Group-10-Item-4", "Spinach",
            "The fresh juice of spinach can make for a powerful natural cure for baldness.",
            "assests/spinach.jpg",
            "",
            "Apply mixture of lettuce and spinach juice regularly on head to prevent hair loss.",
            group10));

            group10.Items.Add(new SampleDataItem("Group-10-Item-5", "Carrot",
            "Carrots are loaded with nutrients in liquid form is a very effective way to realize the benefits of consuming these wonder foods. ",
            "assets/carrot1.jpg",
            "",
            "Drink juice of lettuce or carrots regularly.",
            group10));

            group10.Items.Add(new SampleDataItem("Group-10-Item-6", "Mustard",
            "Mustard seed oil is supposed to stimulate the scalp to promote hair growth. It contains extremely high levels of zinc and selenium. ",
            "assets/mustard.jpg",
            "",
            "Boil one cup of mustard oil with four tablespoon heena leaves. Filter it and keep it in a glass bottle. Massage gently on bald patches regularly.",
            group10));
            this.AllGroups.Add(group10);
            var group11 = new SampleDataGroup("bed wetting",
    "Disease : Bed wetting",
    "Bed weeting is involuntary urination while asleep after the age at Which bladder control usually occurs. ",
    "Assets/bed wetting.jpg",
    "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group11.Items.Add(new SampleDataItem("Group-11-Item-1",
            "Walnut",
            "Walnuts and can also be used to reduce the symptoms of bedwetting. ",
            "Assets/walnuts.jpg",
            "",
            "Eat two walnut halves and 5-6 raisins before going to bed.",
            group11));

            group11.Items.Add(new SampleDataItem("Group-11-Item-2", "Cranberry",
            "Cranberry juice is good for the kidneys, the bladder, and the urinary tract and thus good for bedwetting.",
            "assets/cranberry.jpg",
            "",
            "One hour before going to bed take cranberry juice.",
            group11));
            this.AllGroups.Add(group11);
            var group12 = new SampleDataGroup("black eye",
    "Disease : Black eye",
    "Black eye is bruising around the eye commanly due to an injury to the face rather than eye injury. ",
    "Assets/black eye.jpg",
    "Group Descriion: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group12.Items.Add(new SampleDataItem("Group-12-Item-1",
            "General",
            "",
            "Assets/general.png",
            "",
            "Gently apply an ice pack or cold cloth to the black eye as soon as possible to reduce swelling and bleeding under the skin for 10-15 minutes. Pratice this for next 2-3 days. Apply warm compress after the swelling goes down which may help to relieve pain.",
            group12));
            this.AllGroups.Add(group12);

            var group13 = new SampleDataGroup("blisters",
"Disease : Blisters",
"A blister is small pocket of fluid within the upper layers of skin,typically caused by forceful rubbing,burning,freezing,chemical exposure or infection.",
"Assets/blister.jpg",
"Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group13.Items.Add(new SampleDataItem("Group-13-Item-1", "Petroleum jelly",
                "Petroleum jelly works to coat the area and provide an instant barrier for the skin.",
                "Assets/petroleum.jpg",
                "",
                "Use petroleum jelly to grease your feet when you are wearing a new pair or if you're going on a long walk. Apply the petroleum jelly to the areas of your feet that are most likely to get blisters.",
                group13));
            group13.Items.Add(new SampleDataItem("Group-13-Item-2", "General",
                "",
                "Assets/general.png",
                "",
                "Do not cover the blister unless something rubs against it. If the blister gets some air,it will get cured sooner.",
                group13));
            this.AllGroups.Add(group13);



            var group14 = new SampleDataGroup("boils",
            "Disease : Boils",
            "A boil is most commanly caused by infection resulting in painful swollen area on the skin by an accumulation of pus. ",
            "Assets/boil.jpg",
            "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group14.Items.Add(new SampleDataItem("Group-14-Item-1",
            "Garlic",
            "Garlic work by drawing out toxins, increasing blood circulation, soothing skin inflammation, as well as disinfecting the wound.",
            "Assets/garlic.jpg",
            "",
            "Gently apply juice of garlic or onion externally on boils to help ripen them,break them and evacuate the pus.",
            group14));
            group14.Items.Add(new SampleDataItem("Group-14-Item-2", "Honey",
            "Honey's high sugar content draws fluid out of the microbes, dehydrating them and causing them to die.",
            "Assets/honey_1.jpg",
            "",
            "Put a warm fig or honey poultice to the affected areas.",
            group14));
            group14.Items.Add(new SampleDataItem("Group-14-Item-3", "Cabbage",
            "Raw cabbage is active in ulcerative colitis, hemorrhoids and constipation, especially thanks to its mucilage and its substances that contain cellulose.",
            "Assets/cabbage.jpg",
            "",
            "Apply a hot cabbage leaf poultice,which helps to draw out infection.",
            group14));
            group14.Items.Add(new SampleDataItem("Group-14-Item-4", "Bitter gourd",
            " The most effective juice for treating boils is that which is made of bitter gourd.",
            "Assets/bitter gourd.jpg",
            "",
            "Take a cupful of fresh bitter gourd juice mixed with one teaspoon of limejuice take it on an empty stomach daily for few months.",
            group14));
            group14.Items.Add(new SampleDataItem("Group-14-Item-5", "Cumin",
            "Cumin seeds not only add taste to food but also are very beneficial for body.",
            "Assets/cumin_seeds.jpg",
            "",
            "Grind cumin seeds with water and apply this paste on boils.",
            group14));
            this.AllGroups.Add(group14);

            var group15 = new SampleDataGroup("bruises",
"Disease : Bruises ",
"A bruise is a state of tissue in which capillaries and sometimes venules are damaged by trauma,allowing blood to seep,hemorrhage. ",
"Assets/bruise.jpg",
"Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group15.Items.Add(new SampleDataItem("Group-15-Item-1", "Vinegar",
                        " Vinegar, especially Apple Cider Vinegar is often touted as a cure-all for everything from headaches to acne.", "Assets/vinegar.jpg", "",
            "Apply vinegar to the bruise using a cottan ball. This will speed up the healing.",
            group15));

            this.AllGroups.Add(group15);

            var group16 = new SampleDataGroup("burns",
"Disease : Burns",
"A burn is a type of injury to flesh or skin caused by heat,electricity,chemicals,etc.",
"Assets/burns.jpg",
"Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group16.Items.Add(new SampleDataItem("Group-16-Item-1",
            "General",
            "",
            "Assets/general.png",
            "",
            "Immediately hold the burned area under cold running water for atleast 10minutes. Don't put lotions or creams on the burn. Cover it with any clean bandage loosely.",
            group16));
            group16.Items.Add(new SampleDataItem("Group-16-Item-2", "Aloe vera",
            "Aloe Vera is also a great treatment for nearly any type of burn, including sunburns, burns from hot surfaces and liquids, steam burns and even to some extent radiation burns.",
            "Assets/aloe vera.jpg",
            "",
            "Use aloe vera cream on burn. Mix one table spoon aloe vera with one-fourth teaspoon each of sandalwood and turmeric and apply the paste on burnt area.",
            group16));
            group16.Items.Add(new SampleDataItem("Group-16-Item-3", "Coriander",
            "",
            "Assets/coriander_1.jpg",
            "",
            "Take a fresh juice of coriander by placing a handfull of cilantro in the blender with about one-third cup of water. Strain it and take 2 teaspoons thrice in a day.",
            group16));
            this.AllGroups.Add((group16));


            var group17 = new SampleDataGroup("chapped or dry lips",
"Disease : Chapped or Dry lips",
"Chapped or dry lips is a condition whereby the lips become dry and possibly cracked due to evaporation of moisture. ", "dry lips.jpg",
"Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group17.Items.Add(new SampleDataItem("Group-17-Item-1",
            "Petroleum jelly",
            "Petroleum jelly's effectiveness in accelerating wound healing stems from its sealing effect on cuts and burns, which inhibits germs from getting into the wound and keeps the injured area supple by preventing the skin's moisture from evaporating.",
            "Assets/petroleum.jpg",
            "",
            "Apply petroleum jelly on the chapped or dry lips.",
            group17));
            group17.Items.Add(new SampleDataItem("Group-17-Item-2", "Honey",
            "Honey has the power to moisturize and heal, making it an ideal treatment for parched lips.",
            "Assets/honey_1.jpg",
            "",
            "Mix honey with milk cream and apply before going to bed.",
            group17));
            group17.Items.Add(new SampleDataItem("Group-17-Item-3", "Aloe vera",
            "Aloe vera is rich in vitamin E, hence it works wonders for the lips.",
            "Assets/aloe vera.jpg",
            "",
            "Put some aloe vera gel on your lips in the night,it is very effective.",
            group17));
            group17.Items.Add(new SampleDataItem("Group-17-Item-4", "Neem",
            "Neem may help restore moisture and elasticity to dry, chapped lips.",
            "Assets/tree-neem,jpg",
            "",
            "Apply neem leaf extract on your lips.",
            group17));
            group17.Items.Add(new SampleDataItem("Group-17-Item-5", "Cucumber",
            "They contain helpful properties that treat chapped lips and reducing the dryness. Cucumbers also contain Vitamin A, an essential vitamin needed for skin repair.",
            "Assets/Cucumber.jpg",
            "",
            "Rub cucumber slice on your lips.",
            group17));
            this.AllGroups.Add(group17);
            var group18 = new SampleDataGroup("chicken pox",
            "Disease : Chicken pox",
            "Chicken pox is highly contagious but non-threatening disease caused by primary infection. ",
            "Assets/chicken pox.jpg",
            "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group18.Items.Add(new SampleDataItem("Group-18-Item-1",
            "Baking soda",
            "Baking soda is a popular remedy to control the itching in chicken pox.",
            "Assets/baking soda.jpg",
            "",
            "Add two tablespoon of baking soda to bath water, it will reduce itching.",
            group18));
            group18.Items.Add(new SampleDataItem("Group-18-Item-2", "Oat",
            "A bath of oatmeal is considered a natural remedy for relieving the itch due to chicken pox. ",
            "Assets/oat1.jpg",
            "",
            "A bath of oatmeal is  natural remedy for relieving the itch during chicken pox. Add oatmeal lotion to the bath water.",
            group18));
            group18.Items.Add(new SampleDataItem("Group-18-Item-3", "Neem",
            "Neem reduces the itchy and scratchy sensation that the chicken pox blisters cause.",
            "Assets/tree-neem.jpg",
            "",
            "Lukewarm water baths with neem leaves is very effective in relieving the itching.",
            group18));
            group18.Items.Add(new SampleDataItem("Group-18-Item-4", "Sandalwood",
            "An application of sandalwood oil is considered to be good for chickenpox.",
            "Assets/sandalwood.jpg",
            "",
            "Apply sandalwood oil from the first day of appearance of rash till the fall of scabs.",
            group18));
            group18.Items.Add(new SampleDataItem("Group-18-Item-5", "Honey",
            "The use of honey as an external application has also proved valuable in chicken pox.",
            "Assets/honey_1.jpg",
            "",
            "The skin should be smeared with honey for fast recovery.",
            group18));
            group18.Items.Add(new SampleDataItem("Group-18-Item-6", "Carrot",
            "A soup prepared from carrots has been found beneficial in the treatment of chicken pox.",
            "Assets/carrot1.jpg",
            "",
            "Drink carrot and corriander soup.",
            group18));
            this.AllGroups.Add(group18);

            var group19 = new SampleDataGroup("cold sores",
            "Disease : Cold sores",
            "Cold sores is an outbreak that typically causes a small blisters or sores on or aroung mouth. ",
            "Assets/cold sores.jpg",
            "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group19.Items.Add(new SampleDataItem("Group-19-Item-1", "General",
            "",
            "Assets/general.png",
            "",
            "As soon as you notice tingling,apply a small ice pack to the area,for about 10-15mins every hour. Place a cool,wet towel on the sores three times a day for 20 minutes each time to help reduce swelling and redness.",
            group19));
            group19.Items.Add(new SampleDataItem("Group-19-Item-2", "Tea",
            "A tea bag remedy for cold sores could be the magic bullet you need to get over these pesky outbreaks.",
            "Assets/tea.jpg",
            "",
            "Apply tea tree oil onto a cold sore, which may help the blisters dry out and heal more quickly. Apply a tea bag for few minutes on the blisters,repeat this frequently in a day.",
            group19));
            group19.Items.Add(new SampleDataItem("Group-19-Item-3", "Aloe vera",
           "Aloe vera does, in fact, inactivate a variety of viruses, including the herpes simplex virus that causes cold sores.",
           "Assets/aloe vera.jpg",
           "",
           "Gently rub aloe vera gel on the sores.",
           group19));
            this.AllGroups.Add(group19);

            var group20 = new SampleDataGroup("comman cold",
            "Disease : Comman cold",
               "Comman cold is infectious disease of the upper respiratory tract which affects primarily the nose. ", "Assets/comman cold.jpg",
            "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group20.Items.Add(new SampleDataItem("Group-20-Item-1",
            "Ginger",
            "Ginger tea can help relieve congestion.",
            "Assets/ginger.jpg",
            "",
            "Take ten grams of ginger and cut into small pieces and then boil in a cup of water. Strain it and add half a teaspoon of sugar. Drink it hot.",
            group20));
            group20.Items.Add(new SampleDataItem("Group-20-Item-2", "Lemon",
            "lemon water with honey helps to loosen congestion, prevents dehydration and soothes your throat.",
            "Assets/Lemon.jpg",
            "",
            "Dilute one lemon in a glass of warm water,and add a teaspoon of honey. Drink twice daily.",
            group20));
            group20.Items.Add(new SampleDataItem("Group-20-Item-3", "Cinnamon",
            "Chinese medicine has long used cinnamon as a natural remedy for coughs and the common cold.",
            "Assets/cinnamon_bark.jpg",
            "",
            "Cinnamon is an excellent warming herb,and can be added to food and drinks to ease out symptons.",
            group20));
            group20.Items.Add(new SampleDataItem("Group-20-Item-4", "Honey",
            "Honey loosens up the congestion.",
            "Assets/honey_1.jpg",
            "",
            "Eat fresh honey or add honey to herbal teas which will encourage healing and prevent further infections.",
            group20));
            group20.Items.Add(new SampleDataItem("Group-20-Item-5", "Turmeric",
            " Turmeric relieves the nasal inflammation associated with the common cold.",
            "Assets/turmeric.jpg",
            "",
            "Add half a teaspoon of fresh turmeric powder in 50ml of milk and boil over a slow fire. Drink this milk twice daily.",
            group20));
            group20.Items.Add(new SampleDataItem("Group-20-Item-6", "Garlic",
            "Garlic is popularly believed to be useful for the common cold as it has antibacterial and antiviral properties.",
            "Assets/grlic.jpg",
            "",
            "Eat garlic flakes daily which will develop immunity against cold and will also work to reduce fever.",
            group20));
            this.AllGroups.Add(group20);

            var group21 = new SampleDataGroup("conjunctivitis",
"Disease : Conjunctivitis",
"Conjunctivitis is inflammation of the conjunctiva commanly due to infection or allergic reaction. ", "Assets/conjuctivitis.jpg",
"Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group21.Items.Add(new SampleDataItem("Group-21-Item-1",
            "Gooseberry",
            "Indian gooseberry juice has medicinal properties which cure conjunctivitis.",
            "Assets/gooseberry.jpg",
            "",
            "Take a cup of Indian gooseberry juice mixed with two teaspoons of honey twice in a day.",
            group21));
            group21.Items.Add(new SampleDataItem("Group-21-Item-2", "Coriander",
            "Coriander juice is highly beneficial in deficiencies of vitamin A, B1, B2, C and iron.",
            "Assets/coriander_1.jpg",
            "",
            "Make eyewash by steeping one teaspoon of coriander seeds in one cup of boiling water for atleast 15minutes. Strain and cool it before using.",
            group21));
            group21.Items.Add(new SampleDataItem("Group-21-Item-3", "Turmeric",
            "Extract from turmeric root may help fight infections.",
            "Assets/turmeric.jpg",
            "",
            "Add some turmeric into a half cup pf pure water. Immense a clean cotton cloth into solution, and let it dry. Then use it to mop the affected eyes. This will facilitate healing.",
            group21));
            this.AllGroups.Add(group21);

            var group22 = new SampleDataGroup("constipation",
            "Disease : Constipation",
            "Constipation refers to bowel movements that infrequent or hard to pass. ",
            "Assets/constipation.jpg",
            "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group22.Items.Add(new SampleDataItem("Group-22-Item-1",
            "Apple",
            "As the apple fiber travels through your gastrointestinal tract, it combines with water to form larger, softer stools that pass easily through your bowels and out of your body.",
            "Assets/apple_1.jpg",
           "",
            "Eat a raw apple about an hour after a meal. Apples are very good cleansers and will encourage bowel movements. Pineapple juice,pears,guava are also effective.",
            group22));
            group22.Items.Add(new SampleDataItem("Group-22-Item-2", "Banana",
            "Bananas are very rich in fiber, potassium and manganese. They are also an incredible vitamin C and B6 source.",
            "Assets/banana.jpg",
           "",
            "Eat ripe yellow banana between meals but not during meals.",
            group22));
            group22.Items.Add(new SampleDataItem("Group-22-Item-3", "Honey",
            " Honey helps the intestines and kidneys perform better thus helping combat constipation.",
            "Assets/honey_1.jpg",
            "",
            "Honey has laxative properties and can be added to food or drinks to relieve constipation.",
            group22));
            group22.Items.Add(new SampleDataItem("Group-22-Item-4", "Lemon",
            "Using hot lemon juice rectally in the form of an enema helps to eliminate the buildup of hardened fecal matter and toxins in the colon.",
            "Assets/Lemon.jpg",
            "",
            "Squeeze half a lemon in a glass of hot water,add half a teaspoon of salt in it and drink.",
            group22));
            group22.Items.Add(new SampleDataItem("Group-22-Item-5", "Aloe vera",
            "The antifungal and anti-bacterial properties of aloe vera help in the treatment of internal as well as external problems.",
            "Assets/aloe vera.jpg",
            "",
            "Aloe vera juice can reduce constipation.",
            group22));
            group22.Items.Add(new SampleDataItem("Group-22-Item-6", "Milk products",
            "",
            "Assets/milkproducts.jpg",
            "",
            "For constipation during pregancy,a cup of hot milk with a teaspoon of ghee added is very effective.",
            group22));
            this.AllGroups.Add(group22);

            var group23 = new SampleDataGroup("cough",
            "Disease : Cough",
            "A cough is sudden and often repetitively occuring reflex which helps to clear large breathing passages from secretion,irritants,foriegn particles and microbes. ",
            "Assets/cough.jpg",
            "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group23.Items.Add(new SampleDataItem("Group-23-Item-1", "Clove",
            " Cloves are great for reducing the painful spasmodic cough that sometimes comes with bronchitis, asthma, and tuberculosis.",
            "Assets/clove.jpg",
            "",
            "Take 2-3 cloves fried in ghee and suck.",
            group23));
            group23.Items.Add(new SampleDataItem("Group-23-Item-2", "Banana",
            "For dry coughs and whooping cough, a drink made from bananas can work like an expectorant and help break up the cough.",
            "Assets/banana.jpg",
            "",
            "For dry cough,eat a ripe banana with one teaspoon of honey and two pinches of ground black pepper,twice or thrice daily.",
            group23));
            group23.Items.Add(new SampleDataItem("Group-23-Item-3", "Turmeric",
            "It is very useful with a constant dry tickle in throat. ",
            "Assets/turmeric.jpg",
            "",
            "Boil one cup milk with teaspoon turmeric and teaspoon ginger and drink this at night before going to bed. This will relieve the cough.",
            group23));
            group23.Items.Add(new SampleDataItem("Group-23-Item-4", "Ginger",
            "Ginger is also a natural muscle relaxer that can relieve spasms associated with intense coughing.",
            "Assets/ginger.jpg",
            "",
            "For productive cough,drink a tea made from half teaspoon ginger powder and pinch of cinnamon powder in a cupful of boiled water.",
            group23));
            group23.Items.Add(new SampleDataItem("Group-23-Item-5", "Honey",
            "Honey can help in treating cough to a greater extent.",
            "Assets/honey_1.jpg",
            "",
            "Take honey and lemon which will ese cough and bring healing. For productive cough,mix one-fourth teaspoon of black pepper with one teaspoon of honey and eat it on a full stomach two or three times a day for 3-5 days.For productive cough,take one teaspoon honey mixed with pinch of clove powder,three times a day.",
            group23));
            this.AllGroups.Add(group23);

            var group24 = new SampleDataGroup("dandruff",
            "Disease : Dandruff",
            "Dandruff is the shedding of dead skin cells from the scalp. ",
            "Assets/dandruff.jpg",
            "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group24.Items.Add(new SampleDataItem("Group-24-Item-1",
            "Egg",
            "Egg shampoo is one of the oldest beauty recipes. Eggs are full of protein.",
            "Assets/egg.jpg",
            "",
            "Mix tow egg whites with fresh juice of one lime and apply to your hairs. Wait for half an hour,then wash the hair with neem soap.",
            group24));
            group24.Items.Add(new SampleDataItem("Group-24-Item-2", "Seasame",
            "Massage of seasame oil help to increase blood circulation in scalp and helps to get rid of dandruff.",
            "Assets/seasame.jpg",
            "",
            "Massage your head for a few minutes daily with seasame oil.",
            group24));
            group24.Items.Add(new SampleDataItem("Group-24-Item-3", "Fenugreek",
            "Fenugreek seeds are an excellent remedy to treat the dandruff caused by fungus, because of its anti-fungal properties.",
            "Assets/fenugreek-seeds.jpg",
            "",
            "Two table spoon of fenugreek seeds should be soaked overnight in water and in the morning,ground them into a fine paste.Apply this paste all over the scalp and leave it for half an hour. Wash your hair thoroughly with soap-nut solution or shikakai.",
            group24));
            group24.Items.Add(new SampleDataItem("Group-24-Item-4", "Neem",
            "Neem benefits for Hair are numerous and better results are obtained if you massage you head and hair with neem oil and keep it overnight. ",
            "Assets/tree-neem.jpg",
            "",
            "Make paste of neem leaves and apply to the scalp. Leave it on for half an hour and then wash the hair with lukewarm water.",
            group24));
            this.AllGroups.Add(group24);

            var group25 = new SampleDataGroup("dengue",
            "Disease : Dengue",
            "Dengue fever also known as breakbone fever,is infecttious tropical disease caused by the dengue virus.",
            "Assets/dengue.jpg",
            "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group25.Items.Add(new SampleDataItem("Group-25-Item-1", "Papaya",
            "",
            "Assets/papaya.jpg",
            "",
            "Papaya juice is a natural cure for dengue fever. The juice of papaya leaf is a sure cure for platellete deficiency.",
            group25));
            group25.Items.Add(new SampleDataItem("Group-25-Item-2", "Basil",
            "This bitter and pungent herb has all the properties that strengthen the internal system against fever.",
            "Assets/basil.jpg",
            "",
            "Use basil leaf ten pieces and one black pepper. This should be the proportion. Grind itt and make pea size pills,use it with water.Boiled tulsi leaves served in a warm drink like tea can help prevent outbreak of dengue.",
            group25));
            group25.Items.Add(new SampleDataItem("Group-25-Item-3", "Fenugreek",
            "It helps to sooth down and cleanse the system.",
            "Assets/fenugreek-seeds.jpg",
            "",
            "Fenugreek leaves are taken as herbal tea in order to reduce fevers. This drink acts as a soothing and cleansing tea for the human system.",
            group25));
            group25.Items.Add(new SampleDataItem("Group-25-Item-4", "Garlic",
            "Garlic promotes the well-being of the heart and immune systems with antioxidant properties and helps maintain healthy blood circulation.",
            "Assets/garlic.jpg",
           "",
            "Chewing two cloves of garlic or drinking hot garlic vegetable soup can decrease the multiplication of viruses.",
            group25));
            group25.Items.Add(new SampleDataItem("Group-25-Item-5", "General",
            "",
            "Assets/general.png",
            "",
            "Drink plenty of fluids like oral rehyderation solution,fresh juice,soups,coconut water. This will help to prevent dehydration due to vomiting and high fever.",
            group25));
            this.AllGroups.Add(group25);

            var group26 = new SampleDataGroup("depression",
            "Disease : Depression",
            "Depression is a state of low mood and aversion to activity that can have a negative effect on a person's thoughts.",
            "Assets/depression1.jpg",
            "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group26.Items.Add(new SampleDataItem("Group-26-Item-1",
            "Tea",
            "",
            "Assets/tea.jpg",
            "",
            "Drink ginger tea twice a day.",
            group26));
            group26.Items.Add(new SampleDataItem("Group-26-Item-2", "Apple",
            "A cup of tea eases frazzled nerves, helps your heart, and may even help fight cancer.",
            "Assets/apple_1.jpg",
            "",
            "Try to fast for few days on apple juice.Apple juice is very effective nerve tonic and recharges the nerves with new energy and life.",
            group26));
            group26.Items.Add(new SampleDataItem("Group-26-Item-3", "Coconut",
            " Coconut oil stimulates the thyroid, which in turn can help lower cholesterol and boost metabolism.One symptom of thyroid issues is depression.",
            "Assets/coconut.jpg",
            "",
            "Rub some coconut oil or sunflower oil on the scalp and soles of your feet at bedtime.",
            group26));
            group26.Items.Add(new SampleDataItem("Group-26-Item-4", "Seasame",
            "Sesame oil has been used as a treatment for depression and anxiety and to promote a sense of wellbeing.",
            "Assets/seasame.jpg",
            "",
            "Nose drops of warm seasame oil is  effective for relieving depression.",
            group26));
            this.AllGroups.Add(group26);

            var group27 = new SampleDataGroup("diabetes",
            "Disease : Diabetes",
            "Diabetes is a group of metabolic diseases in which a person has high blood sugar.",
            "Assets/Diabetes.jpg",
            "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group27.Items.Add(new SampleDataItem("Group-27-Item-1",
            "General",
            "",
            "Assets/general.png",
            "",
            "Put one cup of water into a copper vessel overnight and drink the water in the morning on empty stomach.",
            group27));
            group27.Items.Add(new SampleDataItem("Group-27-Item-2", "Gooseberry",
            "Gooseberry stimulates the isolated group of cells which secrete the hormone insulin and reduces blood sugar in diabetic patient.",
            "Assets/gooseberry.jpg",
            "",
            "Take a tablespoon of indian gooseberry juice,mixed with a cup of bitter gourd juice,daliy for two months.",
            group27));
            group27.Items.Add(new SampleDataItem("Group-27-Item-3", "Bitter gourd",
            "Bitter gourd has been used to treat diabetes in traditional medicine. ",
            "Assets/bitter gourd.jpg",
            "",
            "Take the juice of about two or three bitter gourd every morning on an empty stomach.",
            group27));
            group27.Items.Add(new SampleDataItem("Group-27-Item-4", "Aloe vera",
            "The mechanism of action of aloe vera to reduce blood-glucose levels is by enhancing glucose metabolism.",
            "Assets/aloe vera.jpg",
            "",
            "Mix half teaspoon of ground bay leaf and half teaspoon turmeric in one tablespoon aloe vera gel. Take the mixture twice a day before meals.",
            group27));
            group27.Items.Add(new SampleDataItem("Group-27-Item-5", "Turmeric",
            "Recent studies have shown that the antioxidant agents in Turmeric help reduce insulin resistance, which may prevent the onset of Type-2 Diabetes.",
            "Assets/turmeric.jpg",
            "",
            "Take one tablespoon of turmeric powder thrice a day,a few minute before meals.",
            group27));
            this.AllGroups.Add(group27);

            var group28 = new SampleDataGroup("dry skin",
"Disease : Dry skin",
"Dry skin is a condition involving the integumentary system which in most cases can safely be treated with moisturizers.",
"Assets/dry skin.jpg",
"Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");

            group28.Items.Add(new SampleDataItem("Group-27-Item-1", "Oat",
                        "This popular cereal is excellent for addressing dry skin symptoms, including itchiness, rashes, scales, peeling, etc. There is hardly anything that can match the curative action of oatmeal for combating dry skin problems.",
                        "Assets/oat1.jpg",
                        "",
                        "Add 5drops of lavender oil or oat extract to bath water. After the bath,apply aloe vera cream.",
                        group28));

            group28.Items.Add(new SampleDataItem("Group-28-Item-2", "Egg",
                        " Egg protein has an important role to play for repairing of tissues and tightening the skin.",
                        "Assets/egg.jpg",
                        "",
                        "Mix an egg yolk and with a few drops of lime and olive oil. Spread on the face and leave it till the skin feels dry and then wash it off with water.",
                        group28));
            group28.Items.Add(new SampleDataItem("Group-28-Item-3", "Cherry",
                "Cherries are a low-calorie fruit with plenty of excellent nutritional benefits.",
                "Assets/cherry.jpg",
                "",
                "Apply a pulp of fresh cherries to the face at night,before going to bed. Leave it for 15minutes and then rinse it off. This will give you a beautiful complexion and also you will find relief from dry skin.",
                group28));

            group28.Items.Add(new SampleDataItem("Group-28-Item-4", "Tea",
            "You can enjoy the beauty benefits of green tea just by putting it directly onto your skin.",
            "Assets/tea.jpg",
            "",
            "Add 1 drop of tea tree oil to your favourite day or night cream to help moisturize and smooth skin.Tea tree oil has been known to penetrate into skin's cellular level.",
            group28));
            group28.Items.Add(new SampleDataItem("Group-28-Item-5", "Aloe vera",
                       "Aloe vera plant is a lively ingredient in a number of different skin care products that facilitates to treat a wide variety of different problems of skin.",
                       "Assets/aloe vera.jpg",
                       "",
                       "Apply aloe vera gel topically on affected areas. Aloe vera is soothing,healing and moisturizing.",
                       group28));
            this.AllGroups.Add(group28);
            var group29 = new SampleDataGroup("fatigue",
"Disease : Fatigue",
"Fatigue is a subjective feeling of tiredness which is distinct from weakness and has gradual onset.",
"Assets/fatigue1.jpg",
"Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group29.Items.Add(new SampleDataItem("Group-29-Item-1", "Orange",
                       "Orange juice is very high in vitamin C, which boosts the immune system of our body. Vitamin C being high in antioxidants, helps to build and repair the body cells and also prevents any damage.",
                       "Assets/orange.jpg",
                       "",
                       "Drink a glass of fresh orange juice with a pinch of rock salt.",
                       group29));
            group29.Items.Add(new SampleDataItem("Group-29-Item-2", "General",
                                 "",
                                 "Assets/general.png",
                                 "",
                                 "Soak 5 fresh dates in a glass of water overnight. Next morning,mix in a blender and drink.",
                                 group29));
            group29.Items.Add(new SampleDataItem("Group-29-Item-3", "Mango",
                           "mango will keep your body from being dehydrated and provides Anti-Oxidants, Xanthones and necessary vitamins and minerals to the body.",
                           "Assets/mango.jpg",
                           "",
                           "Eat one mango daily and after an hour drink one glass warm milk with a teaspoon of ghee added.",
                           group29));
            this.AllGroups.Add(group29);
            var group30 = new SampleDataGroup("fever",
"Disease : Fever",
"Fever is the most comman medical signs and is characterized by an elevation of body temperature above the normal range. ",
"Assets/fever.jpg",
"Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group30.Items.Add(new SampleDataItem("Group-30-Item-1", "Cilantro",
                                 "Cilantro seeds have been associated with offering a feeling of coolness and also reducing fever.",
                                 "Assets/cilantro.jpg",
                                 "",
                                 "Take two teaspoon of cilantro juice three times a day to help lower the fever.Put handful of cilantro leaves in a blender with about one-third cup water and blend it thoroughly. Strain out the pulp and take out the juice.",
                                 group30));
            group30.Items.Add(new SampleDataItem("Group-30-Item-2", "Cumin",
                              "Diluted cumin water has antiseptic properties and can come to aid for common cold and fevers.",
                              "Assets/Cumin_seeds.jpg",
                              "",
                              "Mix cumin seeds,coriander seedss and fennel seeds in equal proportions. Add one teaspoon of this mixture in a cup of boiling water,keep it for 10minutes,strain and drink.",
                              group30));
            group30.Items.Add(new SampleDataItem("Group-30-Item-3", "Ginger",
                              "Ginger contains a number of compounds which help to combat fever and maintain body metabolism.",
                              "Assets/ginger.jpg",
                              "",
                             "Crush 10grams of raisins and ginger. Boil them in 2cup of water till it reduces to half cup ml. Strain and drink this solution warm.",
                              group30));
            group30.Items.Add(new SampleDataItem("Group-30-Item-4", "Fenugreek",
                                   "The Fenugreek herb has been known to help reduce fever when taken with lemon and honey, since it nourishes the body during an illness.",
                                   "Assets/fenugreek-seeds.jpg",
                                   "",
                                   "Boil powder of 2-3 teaspoon dry roasted fenugreek seeds. Strain the water and add a drop of ghee to it and drink.",
                                   group30));
            this.AllGroups.Add(group30);
            var group31 = new SampleDataGroup("hangover",
"Disease : Hangover",
"Hangover is an experience of various unpleasant physiological effects following heavy consumption of alcoholic beverages.",
"Assets/hangover.jpg",
"Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group31.Items.Add(new SampleDataItem("Group-31-Item-1", "Lemon",
            "Lemon is loaded with vitamin C that helps in decrease the alcohol content in the liver.",
            "assets/Lemon.jpg",
            "",
            "Add one teaspoon lemon juice,half teaspoon sugar and pinch of salt in a glass of water and also add half teaspoon of baking soda before drinking.",
             group31));
            group31.Items.Add(new SampleDataItem("Group-31-Item-2", "Orange",
                "Orange juice interrupts the dehydrating effect of alcohol that redirects water away from your kidneys.",
                "assets/orange.jpg",
                "",
                "Drink a glass of fresh orange juice with one teaspoon lime juice and a pinch of cumin powder.",
                group31));
            group31.Items.Add(new SampleDataItem("Group-31-Item-3", "Banana",
                "The banana helps to calm the stomach. This way your hangover disappears quickly and painlessly.",
                "assets/banana.jpg",
                "",
                "Drink banana milk shake sweetened with honey. Banana calms the stomach and with the help of honey builds the depleted sugar levels,while the milk soothes and rehyderates your system.",
                group31));
            group31.Items.Add(new SampleDataItem("group31-31-Item-4", "Coconut",
                "Coconut water calms upset stomachs and reduces vomiting. Coconut water is an all-natural remedy that is easy on the digestive tract.",
                "assets/coconut.jpg",
                "",
                "Drink coconut water, most ofthe time it is beneficial to reduce hangover.",
                group31));
            var group32 = new SampleDataGroup("head ache",
                "Disease : Head ache",
"A headache is pain anywhere in the region of the head or neck.",
"Assets/headache.jpg",
"Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group32.Items.Add(new SampleDataItem("Group-32-Item-1", "Cinnamon",
                "Cinnamon has been found to be an effective natural remedy for eliminating headaches and migraine relief.",
                "assets/cinnamon_bark.jpg",
                "",
                "For sinus headache,make a paste out of one teaspoon cinnamon and sufficient water and apply on forehead.",
                group32));
            group32.Items.Add(new SampleDataItem("Group-32-Item-2", "Aloe vera",
                "Loaded with beneficial amino acids, aloe vera is most commonly used to treat sunburns topically. Drinking aloe vera juice provides a variety of health benefits.",
                "assets/aloe vera.jpg",
                "",
                "For headache due to hyperacidity,take two tablespoons of aloe vera gel,thrice a day.",
                group32));
            group32.Items.Add(new SampleDataItem("Group-32-Item-3", "Sandalwood",
                "Sandalwood oil is known to be a natural sedative, hence, reduces restlessness, fear, headache and stress and induces calmness and positive thoughts.",
                "assets/sandalwood.jpg",
                "",
                "For headache due to hyperacidity,apply a paste of sandalwood to forehead.",
                group32));
            this.AllGroups.Add(group32);
            var group33 = new SampleDataGroup("high blood cholesterol",
                "Disease : High blood cholestrol",
                "Cholesterol is a waxy, fat-like substance that’s found in all cells of the body.",
                "assets/hbc.jpg",
                "");
            group33.Items.Add(new SampleDataItem("Group-33-Item-1", "Honey",
                "High in minerals such as potassium, calcium and sodium and B complex vitamins, honey is known to be a cholesterol fighter - honey lowers cholesterol in our blood.",
                "assets/honey_1.jpg",
                "",
                "Add one teaspoon of honey and one teaspoon of lime juice in one cup of hot water and drink early morning.",
                group33));
            group33.Items.Add(new SampleDataItem("Group-33-Item-2", "Coriander",
                "A variety of acid compounds contained in coriander, linoleic acid, oleic acid, palmitic acid, stearic acid and ascorbic acid are known to be effective in lowering cholesterol levels in the blood.",
                "assets/coriander_1.jpg",
                "",
                "Boil two tablespoons of dry seeds of coriander in a glass of water and strain the solution after cooling. Take this extract twice a day.",
                group33));
            this.AllGroups.Add(group33);

            var group34 = new SampleDataGroup("jet lag",
                "Disease : Jet lag",
                "Jet lag is a physiological condition which results from alterations to the body's circadian rhythms resulting from rapid long-distance transmeridian (east–west or west–east) travel on a (typically jet) aircraft.",
                "assets/jet lag.jpg",
                "");
            group34.Items.Add(new SampleDataItem("Group-34-Item-1", "Ginger",
                "Studies suggest that ginger may be very effective in reducing symptoms of motion sickness.",
                "assets/ginger.jpg",
                "",
                "An hour before flying,swallow one tablespoon of ginger powder with a cup of water.",
                group34));
            group34.Items.Add(new SampleDataItem("Group-34-Item-2", "Seasame",
                "Seasame oil massaged in your ears & up your nose, before/during air travel also keeps these delicate tissues well lubricated. This greatly helps your sinuses, reduces the effects of noise on the nervous system & helps nourish/settle your mind.",
                "assets/seasame.jpg",
                "",
                "When you land down,rub a little warm seasame oil on your scalp and on soles of your feet. Also, drink one cup of hot milk with a pinch each of rutmeg and ginger.",
                group34));

            this.AllGroups.Add(group34);
        }
    }
}
