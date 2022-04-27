package com.example.ServerExample;

import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.List;

@Service
public class ObjectService {
    private final List<Object> _objects = new ArrayList<>();

    public List<Object> GetAllObjects() {
        return  _objects;
    }

    public Object GetObject(int id) {
        for (Object object : _objects) {
            if (object.getId() == id) {
                return object;
            }
        }

        return null;
    }

    public Object AddObject(Object object) {
        _objects.add(object);
        return object;
    }
}
