using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReportPopup : MonoBehaviour
{
    public static ReportPopup Instance;
    public List<Person> people = new List<Person>();
    public List<PersonItem> personItems = new List<PersonItem>();
    public Transform content;
    public PersonItem item;

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Show()
    {
        ShowPersons();
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        DeletePersons();
        this.gameObject.SetActive(false);
    }

    public void AddPerson(Person person)
    {
        people.Add(person);
    }

    public void ShowPersons()
    {
        foreach (var person in people)
        {
            PersonItem personItem = Instantiate<PersonItem>(item, content);
            personItem.Init(person);
            personItems.Add(personItem);
        }
    }

    public void DeletePersons()
    {
        foreach (var person in personItems)
        {
            Destroy(person.gameObject);
        }
        personItems = new List<PersonItem>();
    }
}

[System.Serializable]
public class Person
{
    public string name;
    public Sprite icon;
    public float focus;
    public int pause;
    public string emotion;
}