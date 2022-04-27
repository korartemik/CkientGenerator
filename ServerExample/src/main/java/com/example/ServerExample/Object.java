package com.example.ServerExample;

public class Object {
    private int Id;
    private int Cost;
    private char Size;

    public Object(int id, int cost, char size) {
        Id = id;
        Cost = cost;
        Size = size;
    }
    public int getSize() {
        return Size;
    }

    public void setSize(char size) {
        this.Size = size;
    }

    public int getId() {
        return Id;
    }

    public void setId(int id) {
        this.Id = id;
    }

    public int getCost() {
        return Cost;
    }

    public void setCost(int cost) {
        this.Cost = cost;
    }
}

