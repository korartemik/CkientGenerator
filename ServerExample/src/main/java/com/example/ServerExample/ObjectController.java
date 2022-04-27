package com.example.ServerExample;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
public class ObjectController {
    @Autowired
    ObjectService service;

    @RequestMapping(value = "/products", method = RequestMethod.GET)
    public ResponseEntity<List<Object>> getAll() {
        return new ResponseEntity<>(service.GetAllObjects(), HttpStatus.OK);
    }

    @RequestMapping(value = "/products/{id}", method = RequestMethod.GET)
    public ResponseEntity<Object> getById(@PathVariable int id) {
        Object object = service.GetObject(id);
        if (object != null) {
            return new ResponseEntity<>(object, HttpStatus.OK);
        } else {
            return new ResponseEntity<>(HttpStatus.BAD_REQUEST);
        }
    }

    @RequestMapping(value = "/products", method = RequestMethod.POST)
    public ResponseEntity<Object> create(@RequestBody Object newProduct) {
        Object object = service.AddObject(newProduct);
        return new ResponseEntity<>(object, HttpStatus.CREATED);
    }

}
